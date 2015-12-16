
DROP PROCEDURE IF EXISTS `sp_page`;
CREATE PROCEDURE `sp_page`(
	query_ varchar(1000),
	fields_ varchar(1000),
	sort_ varchar(100),
	pageSize_ int,
	pageIndex_ int,
	out count_  int
)
COMMENT '·ÖÒ³´æ´¢¹ý³Ì'
BEGIN
 if pageSize_<=1 then 
  set pageSize_=20;
 end if;
 if pageIndex_ < 1 then 
  set pageIndex_ = 1; 
 end if;
 
 set @strsql = concat('select ',fields_,' from ',query_,' order by ',sort_,' limit ',pageIndex_*pageSize_-pageSize_,',',pageSize_); 
 prepare stmtsql from @strsql; 
 execute stmtsql; 
 deallocate prepare stmtsql;
 set @strsqlcount=concat('select count(1) INTO @ROWS_TOTAL from ',query_);
 prepare stmtsqlcount from @strsqlcount; 
 execute stmtsqlcount; 
 set count_=@ROWS_TOTAL; 
END