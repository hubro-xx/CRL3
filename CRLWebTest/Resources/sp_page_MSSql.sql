
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_page]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].sp_page
GO
--表分页
CREATE PROCEDURE [dbo].sp_page
( 
	@query_    nvarchar(max)='',    ----查询 table1 where a=1  
    @fields_ nvarchar(1000)='*',        ----要显示的字段列表
    @sort_    nvarchar(100)='',    ----如果多列排序,一定要带asc或者desc,则@singleSortType排序方法无效
    @pageSize_ int = 10,                ----每页显示的记录个数
    @pageIndex_    int = 1,                ----要显示那一页的记录
    @count_    int =1 output           ----查询到的记录数
) 
--参数传入 @pageSize,@pageIndex
AS
set  nocount  on
declare @sqlTmp nvarchar(max)
declare @sqlGetCount nvarchar(max)
declare @start nvarchar(20) 
declare @end nvarchar(20)
declare @pageCount INT

begin

    --获取记录数
	  set @sqlGetCount = 'select @Counts=count(*) from ' + @query_


    ----取得查询结果总数量-----
    exec sp_executesql @sqlGetCount,N'@Counts int out ',@count_ out
    
    if @count_ = 0
        set @count_ = 1

    --取得分页总数
    set @pageCount=(@count_+@pageSize_-1)/@pageSize_

    /**当前页大于总页数 取最后一页**/
    if @pageIndex_>@pageCount
        set @pageIndex_=@pageCount

	--计算开始结束的行号
	set @start = @pageSize_*(@pageIndex_-1)+1
	set @end = @start+@pageSize_-1 
	
	set @sqlTmp='SELECT * FROM (select '+@fields_+',ROW_NUMBER() OVER ( Order by '+@sort_+' ) AS RowNumber From '+@query_+') T WHERE T.RowNumber BETWEEN '+@start+' AND '+@end+' order by '+ @sort_
	exec sp_executesql @sqlTmp
end
