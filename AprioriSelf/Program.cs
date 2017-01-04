using AprioriSelf.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprioriSelf
{
    class Program
    {

        static void Main(string[] args)
        {
            ExistingDataFromWeb existing = new ExistingDataFromWeb();
            List<Item> AllEnrolledCourse = new List<Item>();
            List<Transaction> AllEnrollments = new List<Transaction>();
            List<LearnerEnroll> enrolls;
            int courseId;

            double support = 5;
            double confidence = 20;

            var students = existing.Users.Where(s => s.Type == "learner").ToList();
            foreach (User current in students)
            {
                enrolls = current.LearnerEnrolls.ToList();
                if (enrolls.Count > 0)
                {
                    Transaction t = new Transaction();
                    foreach(LearnerEnroll le in enrolls)
                    {
                        courseId = le.EnrolledCourseID;
                        Item i = new Item();
                        i.ID = courseId;
                        t.Items.Add(i);
                        if (AllEnrolledCourse.Where(s=>s.ID == courseId).FirstOrDefault() == null)
                        {
                            AllEnrolledCourse.Add(i);
                        }
                    }
                    AllEnrollments.Add(t);
                }
                //Console.WriteLine("\t" + (i + 1) + ". " + users.ElementAt(i).Email + " (" + users.ElementAt(i).Type + ")");
            }
            List<Transaction> AllTransactions = AllEnrollments;
            List<Item> AllItems = AllEnrolledCourse;
            List<Rule> RuleList = new List<Rule>();


            Console.WriteLine("Given Data: ");
            foreach (Transaction t in AllTransactions)
            {
                Console.WriteLine(t.print());
            }

            Console.WriteLine();
            List <ItemsSet> frequent = GetFrequentItemsL1(support, AllItems, AllTransactions);

            List<ItemsSet> candidates = GenerateCandidateSets(frequent, AllTransactions);
            frequent = GetFrequentItems(support, candidates, AllTransactions);

            List<ItemsSet> lastFrequent = frequent;
            do
            {
                lastFrequent = frequent;
                candidates = GenerateCandidateSets(frequent, AllTransactions);
                frequent = GetFrequentItems(support, candidates, AllTransactions);
            } while (candidates.Count != 0 && frequent.Count!=0);


            frequent = lastFrequent;

            //Generating Rules Now
            List<ItemsSet> subSets;
            double calculatedConfidence;
            double whole;
            double sub;
            foreach (ItemsSet itemSet in frequent)
            {
                subSets = GetSubSets(itemSet);
                whole = GetSupport(itemSet, AllTransactions);
                foreach (ItemsSet sb in subSets)
                {
                    sub = GetSupport(sb, AllTransactions);

                    calculatedConfidence = ( whole / sub) * 100;
                    if(calculatedConfidence >= confidence)
                    {
                        Rule r = new Rule();
                        r.Driver.AddRange(sb.Items);
                        r.Indicates.AddRange(getRemainingItems(sb, itemSet));
                        r.confidence = calculatedConfidence;
                        if (!isRuleExists(RuleList, r))
                            RuleList.Add(r);
                    }
                }
                //RuleList = RuleList;
            }

            RuleList = RuleList;
            Console.WriteLine("We have " + RuleList.Count + " rules: ");
            OfferedCours oc;
            var rs = existing.Rules.ToList();
            existing.Rules.RemoveRange(rs);
            foreach (Rule r in RuleList)
            {
                Models.Rule ruleForDb = new Models.Rule();
                List<int> DriversID = new List<int>();
                foreach (Item i in r.Driver)
                {
                    DriversID.Add(i.ID);
                }
                List<int> IndicatesID = new List<int>();
                foreach (Item i in r.Indicates)
                {
                    IndicatesID.Add(i.ID);
                }
                ruleForDb.Confidence = Math.Round(r.confidence, 2);
                ruleForDb.Drivers = JsonConvert.SerializeObject(DriversID);
                ruleForDb.Indicates = JsonConvert.SerializeObject(IndicatesID);
                existing.Rules.Add(ruleForDb);
                //Console.WriteLine("(connfidence: {0}%)", Math.Round(r.confidence,2));
            }
            existing.SaveChanges();


            var rulesInDb = existing.Rules.ToList();
            foreach(Models.Rule r in rulesInDb.OrderByDescending(s=>s.Confidence).ToList())
            {
                List<int> d = JsonConvert.DeserializeObject<List<int>>(r.Drivers);
                foreach (int i in d)
                {
                    Console.Write(i + " ");
                }
                Console.Write("->");
                d = JsonConvert.DeserializeObject<List<int>>(r.Indicates);
                foreach (int i in d)
                {
                    Console.Write(i + " ");
                }
                Console.WriteLine("(connfidence: {0}%)", r.Confidence);
            }

            Console.ReadLine();

        }


        public static bool isRuleExists(List<Rule> ruleList, Rule rule)
        {
            bool exists = false;
            foreach(Rule r in ruleList)
            {
                if (r.Driver.Count == rule.Driver.Count)
                {
                    exists = true;
                    foreach(Item i in rule.Driver)
                    {
                        if(r.Driver.Where(s => s.ID == i.ID).FirstOrDefault() == null)
                        {
                            exists = false;
                        }
                    }
                    if (exists)
                        return true;
                }

            }
            return exists;
        }

        public static List<Item> getRemainingItems(ItemsSet done, ItemsSet All)
        {
            List<Item> remaining = new List<Item>();
            foreach(Item i in All.Items)
            {
                if (done.Items.Where(s => s.ID == i.ID).FirstOrDefault() == null)
                    remaining.Add(i);
            }
            return remaining;
        }

        public static List<ItemsSet> GetFrequentItemsL1(double minSupport, List<Item> allItems, List<Transaction> allTransactions)
        {
            List<ItemsSet> frequentItems = new List<ItemsSet>();
            double count = allTransactions.Count;
            
            foreach(Item item in allItems)
            {
                double support = GetSupportL1(item, allTransactions);   //Calculating support of each and everyitem
                if( ((support/count)*100) >= minSupport)
                {
                    ItemsSet iS = new ItemsSet();
                    iS.Items.Add(item);
                    iS.Support = support;
                    frequentItems.Add(iS);
                }
            }

            return frequentItems;
        }

        public static double GetSupportL1(Item i, List<Transaction> ts)
        {
            double val;
            val = ts.Where(s => 
                            s.Items.Where(b => b.ID == i.ID).ToList().Count > 0)
                            .ToList().Count;
            return val;
        }

        public static List<ItemsSet> GetFrequentItems(double minSupport, List<ItemsSet> ItemsSets, List<Transaction> allTransactions)
        {
            List<ItemsSet> candidates = ItemsSets;
            List<ItemsSet> frequent = new List<ItemsSet>();
            double count = allTransactions.Count;
            double support = 0;
            for (int x =0; x<candidates.Count; x++)
            {
                support = GetSupport(candidates.ElementAt(x), allTransactions);
                if ( ((support / count) * 100) >= minSupport)
                {
                    frequent.Add(candidates.ElementAt(x));
                    candidates.ElementAt(x).Support = support;
                }
            }

            //List<ItemsSet> candidates2 = GenerateCandidateSets(candidates, allTransactions);

            return frequent;
        }
        
        public static double GetSupport(ItemsSet iS, List<Transaction> ts)
        {
            double val = 0;

            foreach(Transaction t in ts)
            {
                if(subset(iS, t.Items))
                {
                    val++;
                }
            }

            return val;
        }

        public static bool subset(ItemsSet a, List<Item> b)
        {
            foreach(Item i in a.Items)
            {
                if(b.Where(s=>s.ID==i.ID).FirstOrDefault() == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static List<ItemsSet> GenerateCandidateSets(List<ItemsSet> ItemsSets, List<Transaction> allTransactions)
        {
            List<ItemsSet> candidates = new List<ItemsSet>();
            List<ItemsSet> SubCandidates = new List<ItemsSet>();
            if (ItemsSets.Count<2)
                return candidates;         
            for(int i =0; i<ItemsSets.Count-1; i++)
            {
                ItemsSet first = ItemsSets.ElementAt(i);
                for(int j=i+1; j<ItemsSets.Count;j++)
                {
                    ItemsSet second = ItemsSets.ElementAt(j);
                    SubCandidates = GenerateCadidatesFromLists(first, second);
                    //candidates.AddRange(SubCandidates);
                    if (candidates.Count == 0)
                        candidates.AddRange(SubCandidates);
                    else
                    {
                        foreach (ItemsSet itemS in SubCandidates)
                        {
                            bool found = false;
                            for (int x= 0; x<candidates.Count; x++)
                            {
                                if (isEqualentsItems(itemS, candidates.ElementAt(x)))
                                {
                                    found = true;
                                }
                            }
                            if(!found)
                                candidates.Add(itemS);
                        }
                    }

                }
            }
            //candidates = candidates.Select(s=>s.Items).Distinct().ToList();
            return candidates;
        }

        public static List<ItemsSet> GenerateCadidatesFromLists(ItemsSet a, ItemsSet b)
        {
            List<ItemsSet> subcandidates = new List<ItemsSet>();
            foreach(Item i in b.Items)
            {
                ItemsSet newSet = new ItemsSet();
                newSet.Items.AddRange(a.Items);
                if(newSet.Items.Where(s=>s.ID == i.ID).FirstOrDefault() == null)
                {
                    newSet.Items.Add(i);
                    subcandidates.Add(newSet);
                }
            }
            return subcandidates;
        }

        public static bool isEqualentsItems(ItemsSet is1, ItemsSet is2)
        {
            if (is1.Items.Count != is2.Items.Count)
                return false;
            foreach(Item i in is1.Items)
            {
                if (is2.Items.Where(s => s.ID == i.ID).FirstOrDefault() == null)
                    return false;
            }
            return true;
        }

        public static List<ItemsSet> GetSubSets(ItemsSet IS)
        {
            List<ItemsSet> returnSet = new List<ItemsSet>();
            double count = Math.Pow(2, IS.Items.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(IS.Items.Count, '0');
                ItemsSet set = new ItemsSet();
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        set.Items.Add(IS.Items.ElementAt(j));
                    }
                }
                if(set.Items.Count != IS.Items.Count)
                returnSet.Add(set);
            }
            return returnSet;
        }
    }
}
