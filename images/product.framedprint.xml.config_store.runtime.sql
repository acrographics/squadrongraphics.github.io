************************************  SQL Statement and parameters for query Products  ************************************

declare @ProductID Int
declare @CustomerLevelID Int
declare @FilterProductsByCustomerLevel VarChar
declare @CustomerLevelFilteringIsAscending VarChar

set @ProductID = 864
set @CustomerLevelID = 0
set @FilterProductsByCustomerLevel = 'false'
set @CustomerLevelFilteringIsAscending = 'false'


SELECT p.*, pv.VariantID, pv.price, isnull(pv.saleprice, 0) saleprice, isnull(pv.colors, '') Colors,
				case when pcl.productid is null then 0 else isnull(e.Price, 0) end ExtendedPrice
                FROM dbo.PRODUCT p (nolock)
                    join dbo.PRODUCTVARIANT pv (nolock) on p.ProductID = pv.ProductID 
                    left join dbo.ProductCustomerLevel pcl (nolock) on p.ProductID = pcl.ProductID
					left join ExtendedPrice e on pv.VariantID=e.VariantID and e.CustomerLevelID=@CustomerLevelID
                where p.ProductID = @ProductID 
                    and pv.isdefault = 1
                    and case 
                            when @FilterProductsByCustomerLevel = 'false' then 1
                            when @CustomerLevelFilteringIsAscending = 'true' and (pcl.CustomerLevelID <= @CustomerLevelID or pcl.CustomerLevelID is null) then 1 
				            when @CustomerLevelID=0 and pcl.CustomerLevelID is null then 1
                            when pcl.CustomerLevelID = @CustomerLevelID  or pcl.CustomerLevelID is null then 1 
                            else 0
                        end  = 1

************************************  SQL Statement and parameters for query KitItems  ************************************

declare @ProductID Int

set @ProductID = 864


SELECT * FROM kitgroup (nolock)
                where ProductID = @ProductID
                ORDER BY DisplayOrder

************************************  SQL Statement and parameters for query Frames  ************************************

declare @frtype VarChar
declare @ProductID Int

set @frtype = 's'
set @ProductID = 864


if @frtype='c'
                  select f.FrameID,f.SKU,f.Name,f.LengthWide FROM fs_Frame f (nolock) where f.Published=1 and f.Custom=1
                  ORDER BY Name
                else if @frtype='s'
                  select f.FrameID,f.SKU,f.Name,f.LengthWide,sf.DeltaPrice as Price,sf.DeltaWeight FROM fs_Frame f (nolock) inner join fs_SelectFrame sf (nolock) on (f.FrameID=sf.FrameID and f.Published=1)
                  where sf.PrintType in (select Dimensions from ProductVariant (nolock) where ProductID=@ProductID)
                  ORDER BY sf.DisplayOrder

************************************  SQL Statement and parameters for query Mats  ************************************

declare @frtype VarChar

set @frtype = 's'


if @frtype='c'
                 select m.MatID,m.SKU,m.Name,m.Color FROM fs_Mat m (nolock) where Published=1
                 ORDER BY Name
               else if @frtype='s'
                 select sm.SelectMatID as MatID,sm.PairSKU as SKU,sm.Name,m1.Color as Color1,m2.Color as Color2,m1.MatID as Mat1ID,m2.MatID as Mat2ID
                 FROM fs_SelectMatPair sm (nolock) inner join fs_Mat m1 (nolock) on (sm.Mat1ID=m1.MatID)
                 inner join fs_Mat m2 (nolock) on (sm.Mat2ID=m2.MatID)
                 order by Name

************************************  SQL Statement and parameters for query MatSizes  ************************************

declare @frtype VarChar
declare @ProductID Int

set @frtype = 's'
set @ProductID = 864


if @frtype='s'
                 select * from fs_SelectMatSize (nolock) 
                 where Dimensions in (select Dimensions from ProductVariant (nolock) where ProductID=@ProductID)

************************************  SQL Statement and parameters for query Glazes  ************************************




select * FROM fs_Glaze (nolock) where Published=1
                ORDER BY Name

************************************  SQL Statement and parameters for query ProductsSections  ************************************

declare @ProductID Int

set @ProductID = 864


SELECT DISTINCT dbo.Section.Name, dbo.Section.ParentSectionID, dbo.Section.SectionID, dbo.ProductSection.ProductID
FROM dbo.Section (nolock) 
LEFT OUTER JOIN dbo.ProductSection (nolock) ON dbo.Section.SectionID = dbo.ProductSection.SectionID
WHERE dbo.ProductSection.ProductID=@ProductID


