set ANSI_NULLS ON
set QUOTED_IDENTIFIER ON
go

CREATE  PROC [dbo].[sp_Paging]
@TableName     NVARCHAR(4000),           	--Table name	Eg:Login
@PrimaryKeys   nvarchar(1000),  	--Primary Keys	Eg:'CompanyCode,UserCode'
@PageIndex int=1,           		--Page index	Eg:1
@PageSize   int=10,            		--Page size		Eg:10
@FieldsShow nvarchar(1000)='',  	--Show fields	Eg:'*' or 'UserCode,Email'
@FieldsOrder nvarchar(1000)='',  	--Order fields	Eg:'TeamCode' or 'TeamCode,Email'
@Where    nvarchar(1000)='' 		--Condition		Eg:''where CompanyCode=''HKG13'' and TeamCode=''GAC'''
AS
SET NOCOUNT ON
declare @PageCount INT,
 @QueryType INT --0 Table or View,1 SQL Query String 
 SELECT @QueryType=0
--Check table name
IF OBJECT_ID(@TableName) IS NULL
BEGIN
	SELECT @QueryType=1
END
IF @QueryType=0
	BEGIN
	IF OBJECTPROPERTY(OBJECT_ID(@TableName),N'IsTable')=0
		AND OBJECTPROPERTY(OBJECT_ID(@TableName),N'IsView')=0
		AND OBJECTPROPERTY(OBJECT_ID(@TableName),N'IsTableFunction')=0
	BEGIN
		RAISERROR(N'"%s"no table',1,16,@TableName)
		RETURN
	END
END
--Check parameters
IF ISNULL(@PrimaryKeys,N'')=''
BEGIN
	RAISERROR(N'Create Parameters...',1,16)
	RETURN
END

--Check parameters
IF ISNULL(@PageIndex,0)<1 SET @PageIndex=1
IF ISNULL(@PageSize,0)<1 SET @PageSize=10
IF ISNULL(@FieldsShow,N'')=N'' SET @FieldsShow=N'*'
IF ISNULL(@FieldsOrder,N'')=N''
	SET @FieldsOrder=N''
ELSE
	SET @FieldsOrder=N'ORDER BY '+LTRIM(@FieldsOrder)
IF ISNULL(@Where,N'')=N''
	SET @Where=N''
ELSE
	SET @Where=N'WHERE ('+@Where+N')'

IF @PageCount IS NULL
BEGIN
	DECLARE @sql nvarchar(4000)
	IF @QueryType=0
	SET @sql=N'SELECT @PageCount=COUNT(*)'
		+N' FROM '+@TableName
		+N' '+@Where
	ELSE
	SET @sql=N'SELECT @PageCount=COUNT(*)'
		+N' FROM ('+@TableName+N') a'
		+N' '+@Where
	EXEC sp_executesql @sql,N'@PageCount int OUTPUT',@PageCount OUTPUT
END

DECLARE @TopN varchar(20),@TopN1 varchar(20)
SELECT @TopN=@PageSize,
	@TopN1=(@PageIndex-1)*@PageSize
--First page
IF @PageIndex=1
	IF @QueryType=0
		EXEC(N'SELECT TOP '+@TopN + N' '+@FieldsShow + N' FROM '+@TableName + N' '+@Where + N' '+@FieldsOrder)
	ELSE
		EXEC(N'SELECT TOP '+@TopN + N' '+@FieldsShow + N' FROM '+N'('+@TableName + N') as a '+@Where + N' '+@FieldsOrder)
ELSE
BEGIN
	IF @QueryType=1
		SELECT @TableName='('+@TableName+') AS'
	--Another name
	IF @FieldsShow=N'*'
		SET @FieldsShow=N'a.*'

	--Create primary keys
	DECLARE @Where1 nvarchar(4000),@Where2 nvarchar(4000),
		@s nvarchar(1000),@Field sysname
	SELECT @Where1=N'',@Where2=N'',@s=@PrimaryKeys
	WHILE CHARINDEX(N',',@s)>0
		SELECT @Field=LEFT(@s,CHARINDEX(N',',@s)-1),
			@s=STUFF(@s,1,CHARINDEX(N',',@s),N''),
			@Where1=@Where1+N' AND a.'+@Field+N'=b.'+@Field,
			@Where2=@Where2+N' AND b.'+@Field+N' IS NULL',
			@Where=REPLACE(@Where,@Field,N'a.'+@Field),
			@FieldsOrder=REPLACE(@FieldsOrder,@Field,N'a.'+@Field),
			@FieldsShow=REPLACE(@FieldsShow,@Field,N'a.'+@Field)
	SELECT @Where=REPLACE(@Where,@s,N'a.'+@s),
		@FieldsOrder=REPLACE(@FieldsOrder,@s,N'a.'+@s),
		@FieldsShow=REPLACE(@FieldsShow,@s,N'a.'+@s),
		@Where1=STUFF(@Where1+N' AND a.'+@s+N'=b.'+@s,1,5,N''),	
		@Where2=CASE
			WHEN @Where='' THEN N'WHERE ('
			ELSE @Where+N' AND ('
			END+N'b.'+@s+N' IS NULL'+@Where2+N')'
	EXEC(N'SELECT TOP '+@TopN
		+N' '+@FieldsShow
		+N' FROM '+@TableName
		+N' a LEFT JOIN(SELECT TOP '+@TopN1
		+N' '+@PrimaryKeys
		+N' FROM '+@TableName
		+N' a '+@Where
		+N' '+@FieldsOrder
		+N')b ON '+@Where1
		+N' '+@Where2
		+N' '+@FieldsOrder)
END
	select @PageCount