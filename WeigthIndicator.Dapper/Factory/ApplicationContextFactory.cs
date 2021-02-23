using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace WeigthIndicator.Dapper
{
	public class ApplicationContextFactory
	{
		private readonly string _connString;
        public ApplicationContextFactory(string connString)
        {
			_connString = connString;
		}
		public IDbConnection CreateConnection()
		{
			return new MySqlConnection(_connString);
		}
	}
}
