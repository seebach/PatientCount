using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientCount
{
    public class Queries
    {
        public const string getFormData = @"select CONVERT(varchar(10), t1.id) as id, t1.Product, t2.value, t3.ytd, t1.Name from

                                            (Select k.id, k.Product, [PCM].[dbo].[FormCategories].Name, [PCM].[dbo].[FormCategories].ShortName from (SELECT Products.[id], [Product]
                                            FROM [PCM].[dbo].[Products]

                                            left join [PCM].[dbo].[ExcludeProductCountry]
                                            on [PCM].[dbo].[Products].id = [ExcludeProductCountry].ProductId and[ExcludeProductCountry].CountryId = {0}
                                            Inner Join
                                    (SELECT[ProductId]
                                      FROM [PCM].[dbo].[productformcountry]
                                      Inner Join [PCM].[dbo].[mappings]
                                      On [PCM].[dbo].[productformcountry].[FormCountryId] = [PCM].[dbo].[mappings].id

                                              where CountryId = {0}
                                                    and FormId = {2}) t2

                                            on[PCM].[dbo].[Products].id = t2.ProductId

                                            where[PCM].[dbo].[Products].Active = 1) as k

                                            Left Join
                                            [PCM].[dbo].[FormCategories]

                                                    ON[PCM].[dbo].[FormCategories].FormId = {2}) as t1

                                            left join

                                            (SELECT
                                            [X] as id,
                                            [Y]

                                            , Sum([Value]) as value
                                            FROM[PCM].[dbo].[FormData]
                                                    Where Period = {1} and FormId = {2} and CountryId = {0}
                                              Group by[X],[Y]) as t2
                                              on
                                              t1.id = t2.id and t2.[Y] = t1.Name


                                            left join

                                            (SELECT
                                                   [X] as id,
                                                   [Y]
                                                  , Sum([Value]) as ytd
                                               FROM[PCM].[dbo].[FormData]
                                                Where Period <= {1}
                                            and FormId = {2} and CountryId = {0}
                                              Group by[X],[Y]) as t3
                                              on
                                              t1.id = t3.id and t3.[Y] = t1.Name
                                              order by id";



        public const string getFormDataCategory = @"select CONVERT(varchar(10), t1.id) as id, t1.Product, t1.ProductType, t2.value, t3.ytd, t1.Name from

                                            (Select k.id, k.Product,k.ProductType, [PCM].[dbo].[FormCategories].Name, [PCM].[dbo].[FormCategories].ShortName from (SELECT Products.[id], [Product], [ProductType]
                                            FROM [PCM].[dbo].[Products]

                                            left join [PCM].[dbo].[ExcludeProductCountry]
                                            on [PCM].[dbo].[Products].id = [ExcludeProductCountry].ProductId and[ExcludeProductCountry].CountryId = {0}
                                            Inner Join
                                    (SELECT[ProductId]
                                      FROM [PCM].[dbo].[productformcountry]
                                      Inner Join [PCM].[dbo].[mappings]
                                      On [PCM].[dbo].[productformcountry].[FormCountryId] = [PCM].[dbo].[mappings].id

                                              where CountryId = {0}
                                                    and FormId = {2}) t2

                                            on[PCM].[dbo].[Products].id = t2.ProductId

                                            where[PCM].[dbo].[Products].Active = 1) as k

                                            Left Join
                                            [PCM].[dbo].[FormCategories]

                                                    ON[PCM].[dbo].[FormCategories].FormId = {2}) as t1

                                            left join

                                            (SELECT
                                            [X] as id,
                                            [Y]

                                            , Sum([Value]) as value
                                            FROM[PCM].[dbo].[FormData]
                                                    Where Period = {1} and FormId = {2} and CountryId = {0}
                                              Group by[X],[Y]) as t2
                                              on
                                              t1.id = t2.id and t2.[Y] = t1.Name


                                            left join

                                            (SELECT
                                                   [X] as id,
                                                   [Y]
                                                  , Sum([Value]) as ytd
                                               FROM[PCM].[dbo].[FormData]
                                                Where Period <= {1}
                                            and FormId = {2} and CountryId = {0}
                                              Group by[X],[Y]) as t3
                                              on
                                              t1.id = t3.id and t3.[Y] = t1.Name
                                              order by ProductType, id";

        public const string getPatientsFormData = @"SELECT 
                                               [Name]
                                              ,[ShortName]
                                              ,CONVERT(varchar(10), t1.Value) as Value	                                               
                                          FROM [PCM].[dbo].[FormCategories]   

                                          Left Join   
 
                                          (SELECT
                                             [Y],
                                             Value
                                           FROM[PCM].[dbo].[FormData]
                                           Where Period = {1} and FormId = {2} and CountryId = {0}
                                           ) as t1

                                           on [Name] = t1.Y
                                           Where [FormCategories].FormId = {2}";

    }
}