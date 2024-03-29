﻿using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class ReestrSettingDataService : IReestrSettingDataService
    {
        private readonly ApplicationContextFactory _applicationContextFactory;

        public ReestrSettingDataService(ApplicationContextFactory applicationContextFactory)
        {
           _applicationContextFactory = applicationContextFactory;
        }
        public async Task<ReestrSetting> CreateReestrSetting(ReestrSetting reestr)
        {        
            using (var connection = _applicationContextFactory.CreateConnection())
            {
                await connection.InsertAsync(reestr);
                return reestr;
            }
        }

        public async Task<ReestrSetting> GetReestrSetting()
        {
            using (var connection = _applicationContextFactory.CreateConnection())
            {
                string query = "SELECT *FROM ReestrSettings as rs " +
                    "LEFT JOIN Recipes as r on rs.RecipeId = r.Id  " +
                    "LEFT JOIN Customers as c on rs.customerid =c.id " +
                    "LEFT JOIN Manufactures as m on rs.manufactureId =m.id " +
                    "LIMIT 1";

               var  reesters = await connection.QueryAsync<ReestrSetting,Recipe,Customer,Manufacture,ReestrSetting>(query,
                   (rs,r,c,m)=> 
                   {
                       rs.CurrentRecipe = r;
                       rs.Customer = c;
                       rs.Manufacture = m;
                       return rs;
                   });
                return reesters.FirstOrDefault();
            }
        }

        public async Task<bool> UpdateReestrSetting(ReestrSetting reestr)
        {
            using (var connection = _applicationContextFactory.CreateConnection())
            {
                return await connection.UpdateAsync(reestr);
            }
        }

        public async Task<bool> UpdateReestrSettingBarrelColumn(ReestrSetting reestrSetting)
        {
            using (var connection = _applicationContextFactory.CreateConnection())
            {
                string query = "UPDATE ReestrSettings SET InitialBarrelNumber =@ibn where id=@id";
                var rowAffected = await connection.ExecuteAsync(query,new {ibn=reestrSetting.InitialBarrelNumber,id=reestrSetting.Id });
                return rowAffected == 1;
            }
        }
    }
}
