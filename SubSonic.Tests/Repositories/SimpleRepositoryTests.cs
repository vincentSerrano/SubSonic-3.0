﻿// 
//   SubSonic - http://subsonicproject.com
// 
//   The contents of this file are subject to the New BSD
//   License (the "License"); you may not use this file
//   except in compliance with the License. You may obtain a copy of
//   the License at http://www.opensource.org/licenses/bsd-license.php
//  
//   Software distributed under the License is distributed on an 
//   "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
//   implied. See the License for the specific language governing
//   rights and limitations under the License.
// 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubSonic.DataProviders;
using SubSonic.Query;
using SubSonic.Repository;
using Xunit;

namespace SubSonic.Tests.Repositories
{
    public class Shwerko
    {
        public int ID { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; }
        public DateTime ElDate { get; set; }
        public decimal SomeNumber { get; set; }
        public int? NullInt { get; set; }
        public decimal? NullSomeNumber { get; set; }
        public DateTime? NullElDate { get; set; }
        public Guid? NullKey { get; set; }
        public int Underscored_Column { get; set; }
    }

    public class Shwerko2 {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public DateTime ElDate { get; set; }
        public decimal SomeNumber { get; set; }
        public int? NullInt { get; set; }
        public decimal? NullSomeNumber { get; set; }
        public DateTime? NullElDate { get; set; }
        public Guid? NullKey { get; set; }
        public int Underscored_Column { get; set; }
    }

    public class DummyForDelete
    {
        public int Id { get; set; }
        public String Name { get; set; }
    }

    public abstract class SimpleRepositoryTests
    {
        private readonly IDataProvider _provider;
        private readonly IRepository _repo;

        public SimpleRepositoryTests(IDataProvider provider)
        {
            _provider = provider;

            _repo = new SimpleRepository(_provider, SimpleRepositoryOptions.RunMigrations);
            try
            {
                var qry = new CodingHorror(_provider, "DROP TABLE Shwerkos").Execute();
            }
            catch { }

            try
            {
                new CodingHorror(_provider, "DROP TABLE DummyForDeletes").Execute();
            }
            catch { }
        }

        private Shwerko CreateTestRecord(Guid key)
        {
            var id = key;

            var item = new Shwerko();
            item.Key = id;
            item.Name = "Charlie";
            item.ElDate = DateTime.Now;
            item.SomeNumber = 1;
            item.NullSomeNumber = 12.3M;
            item.Underscored_Column = 1;
            return item;
        }

        [Fact]
        public void Simple_Repo_Should_Create_Schema_And_Save_Shwerko()
        {
            var id = Guid.NewGuid();
            var item = CreateTestRecord(id);
            _repo.Add(item);

            var count = _repo.Find<Shwerko>(x => x.Key == id).Count;

            Assert.Equal(1, count);
        }

        [Fact]
        public void Simple_Repo_Should_Get_Single()
        {
            var id = Guid.NewGuid();
            var item = CreateTestRecord(id);
            _repo.Add(item);

            var count = _repo.Find<Shwerko>(x => x.Key == id).Count;
            Assert.Equal(1, count);

            item = _repo.Single<Shwerko>(x => x.Key == id);
            Assert.Equal(id, item.Key);
        }

        [Fact]
        public void Simple_Repo_Should_Delete_Single_Shwerko_Item()
        {
            var id = Guid.NewGuid();

            var item = CreateTestRecord(id);
            _repo.Add(item);

            var count = _repo.Find<Shwerko>(x => x.Key == id).Count;
            Assert.Equal(1, count);
            item = _repo.Single<Shwerko>(x => x.Key == id);

            _repo.Delete<Shwerko>(item.ID);
            count = _repo.Find<Shwerko>(x => x.Key == id).Count;
            Assert.Equal(0, count);
        }

        [Fact]
        public void Simple_Repo_Should_Add_Multiple_Shwerko_Item()
        {
            List<Shwerko> list = new List<Shwerko>();

            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();
                var item = CreateTestRecord(id);
                list.Add(item);
            }
            _repo.AddMany(list);
            var count = _repo.Find<Shwerko>(x => x.SomeNumber > 0).Count;
            Assert.Equal(10, count);
        }

        [Fact]
        public void Simple_Repo_Should_Delete_Multiple_Shwerko_Item_With_Expression()
        {
            List<Shwerko> list = new List<Shwerko>();

            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();
                var item = CreateTestRecord(id);
                list.Add(item);
            }
            _repo.AddMany(list);
            var count = _repo.Find<Shwerko>(x => x.SomeNumber > 0).Count;
            Assert.Equal(10, count);

            _repo.DeleteMany<Shwerko>(x => x.SomeNumber > 0);
            count = _repo.Find<Shwerko>(x => x.SomeNumber > 0).Count;
            Assert.Equal(0, count);
        }

        [Fact]
        public void Simple_Repo_Should_Delete_Multiple_Shwerko_Item_With_List()
        {
            List<Shwerko> list = new List<Shwerko>();

            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();
                var item = CreateTestRecord(id);
                list.Add(item);
            }
            _repo.AddMany(list);
            list = _repo.Find<Shwerko>(x => x.SomeNumber > 0).ToList();
            Assert.Equal(10, list.Count);

            _repo.DeleteMany(list);
            var count = _repo.Find<Shwerko>(x => x.SomeNumber > 0).Count;
            Assert.Equal(0, count);
        }

        [Fact]
        public void Simple_Repo_Should_Update_Single_Shwerko_Item()
        {
            var id = Guid.NewGuid();
            var item = CreateTestRecord(id);
            _repo.Add(item);

            var count = _repo.Find<Shwerko>(x => x.Key == id).Count;
            Assert.Equal(1, count);

            item.Name = "Updated";
            _repo.Update(item);

            item = _repo.Single<Shwerko>(x => x.Key == id);
            Assert.Equal(id, item.Key);
        }

        [Fact]
        public void Simple_Repo_Should_Update_Multiple_Shwerko_Item()
        {
            List<Shwerko> list = new List<Shwerko>();

            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();
                var item = CreateTestRecord(id);
                list.Add(item);
            }
            _repo.AddMany(list);
            var count = _repo.Find<Shwerko>(x => x.SomeNumber > 0).Count;
            Assert.Equal(10, count);

            //pull it back to synch up the IDs
            list = _repo.Find<Shwerko>(x => x.SomeNumber > 0).ToList();

            //update the prices to 200
            list.ForEach(x => x.Name = "Updated");

            _repo.UpdateMany(list);
            list = _repo.Find<Shwerko>(x => x.SomeNumber > 0).ToList();

            Assert.Equal(list[0].Name, "Updated");
        }

        [Fact]
        public void Simple_Repo_Should_Create_IQueryable()
        {
            List<Shwerko> list = new List<Shwerko>();

            var result = _repo.All<Shwerko>();
            Assert.NotNull(result);
        }

        [Fact]
        public void Simple_Repo_All_Should_Have_Count_10()
        {
            List<Shwerko> list = new List<Shwerko>();
            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();
                var item = CreateTestRecord(id);
                list.Add(item);

            }
            _repo.AddMany(list);
            var count = _repo.Find<Shwerko>(x => x.SomeNumber > 0).Count;
            Assert.Equal(10, count);
            var result = _repo.All<Shwerko>();
            Assert.Equal(10, result.Count());
        }

        [Fact]
        public void Simple_Repo_Exists_Should_Be_True_With_Name_Charlie_False_With_Name_Chuck()
        {
            List<Shwerko> list = new List<Shwerko>();
            for (int i = 0; i < 10; i++)
            {
                var id = Guid.NewGuid();
                var item = CreateTestRecord(id);
                list.Add(item);

            }
            _repo.AddMany(list);

            Assert.True(_repo.Exists<Shwerko>(x => x.Name == "Charlie"));
            Assert.False(_repo.Exists<Shwerko>(x => x.Name == "Chuck"));

        }


        [Fact]
        public void SimpleRepo_Should_Set_The_Guid_PK_On_Add() {
            var item = new Shwerko2();
            var id = Guid.NewGuid();
            item.ID = id;
            item.Name = "Charlie";
            item.ElDate = DateTime.Now;
            item.SomeNumber = 1;
            item.NullSomeNumber = 12.3M;
            item.Underscored_Column = 1;
            _repo.Add<Shwerko2>(item);
            Assert.Equal(item.ID, id);
            //Assert.True(shwerko.ID > 0);
        }


        [Fact]
        public void SimpleRepo_Should_Set_The_PK_On_Add() {
            var shwerko = CreateTestRecord(Guid.NewGuid());
            _repo.Add<Shwerko>(shwerko);
            Assert.True(shwerko.ID > 0);
        }

        [Fact]
        public void Simple_Repo_Should_Run_Migrations_Before_DeleteMany()
        {
            _repo.DeleteMany<DummyForDelete>(x => true);

            var existing = _repo.All<DummyForDelete>().Count();
            Assert.Equal(0, existing);
        }

        [Fact]
        public void Simple_Repo_Should_Run_Migrations_Before_DeleteMany_WithItemsCollection()
        {
            var toDelete = new DummyForDelete[] 
            {
                new DummyForDelete 
                {
                    Id = 1,
                    Name = "AName"
                },
                new DummyForDelete 
                {
                    Id = 2,
                    Name = "BName"
                }
            };

            _repo.DeleteMany(toDelete);

            var existing = _repo.All<DummyForDelete>().Count();
            Assert.Equal(0, existing);
        }

        [Fact]
        public void Simple_Repo_Should_Run_Migrations_Before_Delete()
        {
            _repo.Delete<DummyForDelete>(10);

            var existing = _repo.All<DummyForDelete>().Count();
            Assert.Equal(0, existing);
        }

        [Fact]
        public void Simple_Repo_GetPaged_Should_Allow_Descending_Order()
        {
            GivenShwerkosWithNames("aaa", "bbb", "a");

            var paged = _repo.GetPaged<Shwerko>("Name desc", 0, 1);
            Assert.Equal("bbb", paged[0].Name);
        }

        [Fact]
        public void Simple_Repo_GetPaged_Should_Not_Expect_Case_Sensitive_Order()
        {
            GivenShwerkosWithNames("aaa", "bbb", "ccc");
            var paged = _repo.GetPaged<Shwerko>("Name DESC", 0, 10);
            Assert.Equal("ccc", paged[0].Name);
            Assert.Equal("bbb", paged[1].Name);
            Assert.Equal("aaa", paged[2].Name);
        }

        private void GivenShwerkosWithNames(params string[] names)
        {
            var shwerkos = names.Select(x => new Shwerko { Name = x, ElDate = new DateTime(2010, 1, 1) });

            _repo.AddMany(shwerkos);
        }

		[Fact]
		public void Simple_Repo_Should_Return_Integer_For_Integer_Autogenerated_Keys()
		{

			var newShwerko = new Shwerko
				{
					Name = "Just Persist me",
					ElDate = new DateTime(2010, 1, 1)
				};

			Assert.IsType(typeof (int), _repo.Add(newShwerko));
		}

		[Fact]
		public void Simple_Repo_Should_Implement_String_StartsWith()
		{
			// Arrange
			_repo.Add<Shwerko>(CreateTestRecord(Guid.NewGuid()));

			// Act
			Shwerko shwerko = _repo.Find<Shwerko>(s => s.Name.StartsWith("c")).FirstOrDefault();

			// Assert	
			Assert.NotNull(shwerko);
		}

		[Fact]
		public void Simple_Repo_Should_Implement_String_Contains()
		{
			// Arrange
			_repo.Add<Shwerko>(CreateTestRecord(Guid.NewGuid()));

			// Act
			Shwerko shwerko = _repo.Find<Shwerko>(s => s.Name.Contains("a")).FirstOrDefault();

			// Assert
			Assert.NotNull(shwerko);
		}
    }
}