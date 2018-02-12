using EFC.SamuraiApp.Data;
using EFC.SamuraiApp.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFC.SamuraiApp.Console
{
    public static class SamuraiData
    {
        #region Fields

        private static SamuraiContext _ctx = new SamuraiContext();

        #endregion Fields


        #region Inserts

        public static void InsertSamurai()
        {
            var samurai = new Samurai()
            {
                Name = "New Samurai"
            };

            using (var ctx = new SamuraiContext())
            {
                ctx.Samurais.Add(samurai);

                ctx.SaveChanges();
            }
        }
        
        public static void InsertMultipleSamurais()
        {
            var s1 = new Samurai
            {
                Name = "Takeshi"
            };

            var s2 = new Samurai
            {
                Name = "Reiling"
            };

            using (var ctx = new SamuraiContext())
            {
                ctx.Samurais.AddRange(s1, s2);

                ctx.SaveChanges();
            }
        }
        
        public static void InsertMultipleDifferentObjs()
        {
            var s1 = new Samurai { Name = "Nobunaga" };
            var b1 = new Battle
            {
                Name = "Legendary nobunaga battle",
                StartDate = new DateTime(1889, 01, 01),
                EndDate = new DateTime(1889, 12, 31)
            };

            using (var ctx = new SamuraiContext())
            {
                ctx.AddRange(s1, b1);

                ctx.SaveChanges();
            }
        }

        public static void InsertMultipleDifferentRelatedObjs()
        {
            var q1 = new Quote { Text = "A samurai must codes." };

            var s1 = new Samurai
            {
                Name = "Coder Samurai",
                Quotes = new List<Quote>() { q1 }
            };

            using (var ctx = new SamuraiContext())
            {
                ctx.Add(s1);

                ctx.SaveChanges();
            }
        }

        public static void InsertNewSamuraiToAnExistentBattle()
        {
            var firstBattle = _ctx.Battles.FirstOrDefault();

            var bob = new Samurai
            {
                Name = "Bob",
                SamuraiBattles = new List<SamuraiBattle>()
                {
                    new SamuraiBattle
                    {
                        Battle = firstBattle
                    }
                }
            };

            _ctx.Add(bob);

            _ctx.SaveChanges();
        }

        public static void InsertASecretSamuraiWithNewBattle()
        {
            var i = new SecretIdentity { RealName = "Fernando" };

            var s = new Samurai
            {
                Name = "Yusuke",
                SecretIdentity = i
            };

            var b = new Battle
            {
                Name = "Code battle",
                StartDate = new DateTime(2002, 09, 07),
                EndDate = new DateTime(2019, 12, 30),
                SamuraiBattles = new List<SamuraiBattle>()
                {
                    new SamuraiBattle { Samurai = s }
                }
            };

            _ctx.AddRange(i, s, b);

            _ctx.SaveChanges();
        }

        /// <summary>
        /// When using a not tracked object, we'd better use its FKs!
        /// </summary>
        /// <param name="samraiId"></param>
        public static void AddChildToExistingObjectWhileNotTracked(int samuraiId)
        {
            var q = new Quote
            {
                Text = "I must know my parent key, to create a related (and not tracked) data.",
                SamuraiId = samuraiId
            };

            using (var otherCtx = new SamuraiContext())
            {
                otherCtx.Quotes.Add(q);

                otherCtx.SaveChanges();
            }
        }

        #endregion Inserts


        #region Updates

        public static void UpdateSamuraiLastName()
        {
            var s1 = _ctx.Samurais.FirstOrDefault(s => s.Name.Equals("yusuke"));
            if (s1 != null)
            {
                s1.Name += " Urameshi";

                _ctx.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("No samurai found.");
            }
        }

        public static void UpdateRelatedDataWhenNotTracked()
        {
            var s1 = _ctx.Samurais
                         .Include(s => s.Quotes)
                         .Where(s => s.Quotes.Count() > 0)
                         .FirstOrDefault();

            var q = s1.Quotes.FirstOrDefault();

            if (q != null)
            {
                q.Text += " Did you hear that?!";

                using (var otherCtx = new SamuraiContext())
                {
                    // EF will keep track of s1 and will update it togheter q
                    // S1 is not tracked, like using .AsNoTracking() 
                    // DbContext instance ChangeTracker.StateManager.ChangedCount will be 2
                    otherCtx.Quotes.Update(q);
                    
                    otherCtx.SaveChanges();
                }
            }
        }

        public static void UpdateRelatedDataWithNoTrackingCorrectly()
        {
            var s1 = _ctx.Samurais
                         .Include(s => s.Quotes)
                         .Where(s => s.Quotes.Count() > 0)
                         .FirstOrDefault();

            var q = s1.Quotes.FirstOrDefault();

            if (q != null)
            {
                q.Text += " Did you hear that?!";

                using (var otherCtx = new SamuraiContext())
                {
                    otherCtx.Entry(q).State = EntityState.Modified;

                    // At this time EF will update ONLY quote
                    // ChangeTracker.StateManager.ChangedCount will be 1
                    otherCtx.SaveChanges();
                }
            }
        }

        #endregion Updates


        #region Deletes

        public static void DeleteSomeNobus()
        {
            var nobus = _ctx.Samurais.Where(s => EF.Functions.Like(s.Name, "%nobu%")).ToList();

            if (nobus.Any())
            {
                _ctx.Samurais.RemoveRange(nobus);

                _ctx.SaveChanges();
            }
            else
            {
                System.Console.WriteLine("No nobus found.");
            }
        }

        public static void DeleteSamuraiById(int id)
        {
            var s = _ctx.Samurais.Find(id);

            if (s == null)
            {
                System.Console.WriteLine($"Not found Samurai id = {id}");
                return;
            }
            
            _ctx.Remove(s);
            _ctx.SaveChanges();
        }
        
        public static void DeleteByStoredProcedure(int samuraiId)
        {
            var sql = "EXEC DeleteById {0}";

            try
            {
            var r = _ctx.Database.ExecuteSqlCommand(sql, samuraiId);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"An exception occurred, details: {ex.Message}");
            }
        }

        #endregion Deletes


        #region Queries

        public static void QueryASecretSamurai()
        {
            var f = _ctx.Samurais
                        .Where(s => EF.Functions.Like(s.SecretIdentity.RealName, "Fernando%"))
                        .FirstOrDefault();

            System.Console.WriteLine($"Samurai name: {f?.Name}");
        }

        public static IEnumerable<Samurai> EagerLoadingSamuraiWithQuotes()
        {
            // When there are some grandchild relationship, use .ThenInclude(parent = parent.GrandChild)
            var samuraiWithQuotes = _ctx.Samurais
                                        .Include(s => s.Quotes)
                                        .ToList();

            // Here we have ALL samurais, who has some quote it, will be loaded too
            return samuraiWithQuotes;
        }


        public static IEnumerable<Samurai> LoadingSamuraiWithQuotes()
        {
            var samuraiWithQuotes = _ctx.Samurais
                                        //.Include(s => s.Quotes)
                                        .Where(s => s.Quotes.Count() > 0)
                                        .ToList();

            // At this point, with no Include Quotes, loaded Samurai won't have Quotes loaded, but with a new 
            // query that load its quotes, it will populate in memory
            var samuraiIds = samuraiWithQuotes.Select(s => s.Id).ToList();

            _ctx.Quotes.Where(q => samuraiIds.Contains(q.SamuraiId))
                       .ToList();

            // Now, samuraiWithQuotes have its quotes
            return samuraiWithQuotes;
        }

        #endregion Queries
    }
}
