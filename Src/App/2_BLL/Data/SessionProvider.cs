using System;
using System.Configuration;
using System.IO;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using Core.Entities;
using NHibernate;

using NHibernate.Tool.hbm2ddl;

namespace Core.Data
{
    
    public interface ISessionProvider
    {
        ISession Create();

    }
    public class SessionProvider:ISessionProvider
    {
        private ISessionFactory _factory;
        //public SessionProvider()
        //{
        //    _factory = Fluently.Configure().
        //        Database(MsSqlConfiguration.MsSql2005.ConnectionString(MSSQLConnectionString).ShowSql().AdoNetBatchSize(10))
        //        .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Person>()
        //            .Where(t => t.Namespace == "Core.Entities").Conventions.AddFromAssemblyOf<Person>()
        //            .Setup(convention =>
        //            {
        //                //convention.IsComponentType =
        //                //    type => type == typeof(Name);
        //                //convention.IsComponentType =
        //                //   type => type == typeof(Money);
        //            })))
        //            .ExposeConfiguration(BuildSchema).BuildSessionFactory();
        //}
        //public SessionProvider()
        //{
        //            static Configuration config;
        //public const string DB_FILE = @"..\..\..\db\firstProject.db";
        string dbDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
        string dbFile;
        public SessionProvider()
        {

            dbFile = Path.Combine(dbDirectory, @"db\firstProject.db");


            _factory=Fluently.Configure().
                Database(GetDatabaseConfig())
                .Mappings(SetMappings)
                .ExposeConfiguration(BuildSchema).BuildSessionFactory();
			PopulateDB();
            
        }
        public void SetMappings(MappingConfiguration m)
        {
            m.AutoMappings.Add(GetModel());      
        }
        public AutoPersistenceModel GetModel()
        {
            AutoPersistenceModel model=AutoMap.AssemblyOf<Product>()
                .Where(t => (t.Namespace == "Core.Entities" || t.Namespace == "Core.ValueTypes")).Conventions.AddFromAssemblyOf<Product>()
                .Setup(convention =>
                {
                //    convention.IsComponentType =
                //        type => type == typeof(Name);
                });
            model.CompileMappings();
            model.WriteMappingsTo( GetDataBaseDirectoryPath() );
            return model;
        }

        public IPersistenceConfigurer GetDatabaseConfig()
        {
            if (IsMsqSql())
                return MsSqlConfiguration.MsSql2005.ConnectionString(MSSQLConnectionString).ShowSql().AdoNetBatchSize(0);
            else
                return SQLiteConfiguration.Standard.UsingFile(dbFile).ShowSql();
        }

        private static bool IsMsqSql()
        {
            return ConfigurationManager.AppSettings["UseMsSQL"] == "true";
        }

        public static string MSSQLConnectionString
        {
            get            
			{
				return ConfigurationManager.ConnectionStrings["Default"].ConnectionString.Replace("{0}", GetDataBaseFilePath());
            }
        }
		public static string GetDataBaseFilePath()
		{
			return Path.Combine(GetDataBaseDirectoryPath(),
			ConfigurationManager.AppSettings["DatabaseDirectory"].Replace(@"..\",""));
		}
		public static string GetDataBaseDirectoryPath()
		{
			DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
			string virtualPath = ConfigurationManager.AppSettings["DatabaseDirectory"];
			string parentDir = @"..\";
			while (virtualPath.StartsWith(parentDir))
			{
				dir = dir.Parent;
				virtualPath = virtualPath.Remove(0, parentDir.Length);
			}
			return dir.FullName;
		}

			


        private void BuildSchema(NHibernate.Cfg.Configuration config)
        {
           
            //NHibernateExplanationTestSQLLite.config = config;
            // delete the existing db on each run
            if (IsMsqSql())
            {
                Console.WriteLine(dbFile);
                if (File.Exists(dbFile))
                    File.Delete(dbFile);
				new SchemaExport(config).Drop(true, true);
                new SchemaExport(config).Create(true, true);
				
                
            }
            
           
        }
        public void  PopulateDB()
        {

           
//            var persons = new PersonInMemoryRepository().GetAll();
//            using (var session = _factory.OpenSession())
//            {
//                using(var tx=session.BeginTransaction())
//                {
//                    foreach (Person person in persons)
//                    {
//                        session.Save(person);
//                    }
//                    tx.Commit();
//                }
//            }
        }

        public ISessionFactory Factory
        {
            get { return _factory; }
        }

        public ISession Create()
        {

            return Factory.OpenSession();

        }
    }
}
