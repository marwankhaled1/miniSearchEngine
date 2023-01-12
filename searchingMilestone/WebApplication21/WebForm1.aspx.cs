using Searcharoo.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace WebApplication21
{



    public partial class WebForm1 : System.Web.UI.Page
    {
        static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marwan\source\repos\crawlerTaskIR\crawlerTaskIR\simpleDatabase.mdf; Integrated Security = True";
        static SqlConnection conn = new SqlConnection(connString);
        static string queryText = "";
        static List<int> diff;
        static int numOfWords;
        static DataTable dt;
        SqlDataReader dr;
        PorterStemmer stemmer = new PorterStemmer();
        List<termDocPos> terms = new List<termDocPos>();
        //holds term and hastable for each term contains doc id and positions


        //static Dictionary<string,List<Dictionary<int, List<int>>>>results =new Dictionary<string,List<Dictionary<int,List<int>>>>();
        static Dictionary<string, Dictionary<int, List<int>>> result;
        static Dictionary<int, List<List<int>>> interSections;
        static Dictionary<int, int> docsRank;
        static List<int> orderWords;
        static List<int> Wordsafterorder;

        int mini = int.MaxValue;
        List<string> termPoslist = new List<string>();
        static string print = "";
        static List<int> templist;
        string[] query;

        protected void searchBtn_Click(object sender, EventArgs e)
        {
            dt = new DataTable();
            dt.Clear();
            GridView1.DataSource = dt.DefaultView;
            GridView1.DataBind();

            queryText = TextBox1.Text.ToString();
            if (queryText.Trim() == "")
            {
                //no Query
                return;
            }

            // applyLangOperations(queryText);


            //check if string between --> " "  
            if (queryText[0].ToString() == ("\"") && queryText[queryText.Length - 1].ToString() == "\"")
            {
                //  queryText=queryText.Replace("\\","");

                // apply language operations 
                //get query words from database
                numOfWords = getTermsFromDatabase(applyLangOperations(queryText));
                if (numOfWords == 1)
                {
                    //to build the dictionary
                    setDictionary();

                    multiKeywordSearch();
                    return;
                }

                //to build the dictionary
                setDictionary();


                exactSearch();

            }
            else
            {
                // apply language operations 
                //get query words from database
                numOfWords = getTermsFromDatabase(applyLangOperations(queryText));


                //to build the dictionary
                setDictionary();

                //Mutli-keywords search
                multiKeywordSearch();
            }

        }


        protected List<string> applyLangOperations(string queryText)
        {
            string text = "";
            List<string> words = new List<string>();

            //case folding
            //remove non english characters
            //replace multi spaces with one space 
            //remove numbers from line
            //tokenzing 
            //remove punctuation characters
            //stemming

            // Case folding
            text = queryText.Trim().ToLower();

            // Remove non English character and numbers
            text = Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);

            //replace multi spaces with one space
            text = text.Replace("\n", " ");

            //remove numbers from lines
            text = Regex.Replace(text, @"[\d-]", string.Empty);

            //tokenzing
            string[] tokens = text.Split(new[] { ',', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);

            //remove punctuation characters
            string[] noPuncTockens = new string[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                noPuncTockens[i] = new string(tokens[i].Where(c => !char.IsPunctuation(c)).ToArray());
            }


            ////stemming
            for (int i = 0; i < noPuncTockens.Length; i++)
            {
                noPuncTockens[i] = stemmer.StemWord(noPuncTockens[i]);
                words.Add(noPuncTockens[i]);
            }
            string[] stopwords ={
  "a",
  "able",
  "about",
  "above",
  "abst",
  "accordance",
  "according",
  "accordingly",
  "across",
  "act",
  "actually",
  "added",
  "adj",
  "affected",
  "affecting",
  "affects",
  "after",
  "afterwards",
  "again",
  "against",
  "ah",
  "all",
  "almost",
  "alone",
  "along",
  "already",
  "also",
  "although",
  "always",
  "am",
  "among",
  "amongst",
  "an",
  "and",
  "+",
  "",
  "announce",
  "another",
  "any",
  "anybody",
  "anyhow",
  "anymore",
  "anyone",
  "anything",
  "anyway",
  "anyways",
  "anywhere",
  "apparently",
  "approximately",
  "are",
  "aren",
  "arent",
  "arise",
  "around",
  "as",
  "aside",
  "ask",
  "asking",
  "at",
  "auth",
  "available",
  "away",
  "awfully",
  "b",
  "back",
  "be",
  "became",
  "because",
  "become",
  "becomes",
  "becoming",
  "been",
  "before",
  "beforehand",
  "begin",
  "beginning",
  "beginnings",
  "begins",
  "behind",
  "being",
  "believe",
  "below",
  "beside",
  "besides",
  "between",
  "beyond",
  "biol",
  "both",
  "brief",
  "briefly",
  "but",
  "by",
  "c",
  "ca",
  "came",
  "can",
  "cannot",
  "can't",
  "cause",
  "causes",
  "certain",
  "certainly",
  "co",
  "com",
  "come",
  "comes",
  "contain",
  "containing",
  "contains",
  "could",
  "couldnt",
  "d",
  "date",
  "did",
  "didn't",
  "different",
  "do",
  "does",
  "doesn't",
  "doing",
  "done",
  "don't",
  "down",
  "downwards",
  "due",
  "during",
  "e",
  "each",
  "ed",
  "edu",
  "effect",
  "eg",
  "eight",
  "eighty",
  "either",
  "else",
  "elsewhere",
  "end",
  "ending",
  "enough",
  "especially",
  "et",
  "et-al",
  "etc",
  "even",
  "ever",
  "every",
  "everybody",
  "everyone",
  "everything",
  "everywhere",
  "ex",
  "except",
  "f",
  "far",
  "few",
  "ff",
  "fifth",
  "first",
  "five",
  "fix",
  "followed",
  "following",
  "follows",
  "for",
  "former",
  "formerly",
  "forth",
  "found",
  "four",
  "from",
  "further",
  "furthermore",
  "g",
  "gave",
  "get",
  "gets",
  "getting",
  "give",
  "given",
  "gives",
  "giving",
  "go",
  "goes",
  "gone",
  "got",
  "gotten",
  "h",
  "had",
  "happens",
  "hardly",
  "has",
  "hasn't",
  "have",
  "haven't",
  "having",
  "he",
  "hed",
  "hence",
  "her",
  "here",
  "hereafter",
  "hereby",
  "herein",
  "heres",
  "hereupon",
  "hers",
  "herself",
  "hes",
  "hi",
  "hid",
  "him",
  "himself",
  "his",
  "hither",
  "home",
  "how",
  "howbeit",
  "however",
  "hundred",
  "i",
  "id",
  "ie",
  "if",
  "i'll",
  "im",
  "immediate",
  "immediately",
  "importance",
  "important",
  "in",
  "inc",
  "indeed",
  "index",
  "information",
  "instead",
  "into",
  "invention",
  "inward",
  "is",
  "isn't",
  "it",
  "itd",
  "it'll",
  "its",
  "itself",
  "i've",
  "j",
  "just",
  "k",
  "keep	keeps",
  "kept",
  "kg",
  "km",
  "know",
  "known",
  "knows",
  "l",
  "largely",
  "last",
  "lately",
  "later",
  "latter",
  "latterly",
  "least",
  "less",
  "lest",
  "let",
  "lets",
  "like",
  "liked",
  "likely",
  "line",
  "little",
  "'ll",
  "look",
  "looking",
  "looks",
  "ltd",
  "m",
  "made",
  "mainly",
  "make",
  "makes",
  "many",
  "may",
  "maybe",
  "me",
  "mean",
  "means",
  "meantime",
  "meanwhile",
  "merely",
  "mg",
  "might",
  "million",
  "miss",
  "ml",
  "more",
  "moreover",
  "most",
  "mostly",
  "mr",
  "mrs",
  "much",
  "mug",
  "must",
  "my",
  "myself",
  "n",
  "na",
  "name",
  "namely",
  "nay",
  "nd",
  "near",
  "nearly",
  "necessarily",
  "necessary",
  "need",
  "needs",
  "neither",
  "never",
  "nevertheless",
  "new",
  "next",
  "nine",
  "ninety",
  "no",
  "nobody",
  "non",
  "none",
  "nonetheless",
  "noone",
  "nor",
  "normally",
  "nos",
  "not",
  "noted",
  "nothing",
  "now",
  "nowhere",
  "o",
  "obtain",
  "obtained",
  "obviously",
  "of",
  "off",
  "often",
  "oh",
  "ok",
  "okay",
  "old",
  "omitted",
  "on",
  "once",
  "one",
  "ones",
  "only",
  "onto",
  "or",
  "ord",
  "other",
  "others",
  "otherwise",
  "ought",
  "our",
  "ours",
  "ourselves",
  "out",
  "outside",
  "over",
  "overall",
  "owing",
  "own",
  "p",
  "page",
  "pages",
  "part",
  "particular",
  "particularly",
  "past",
  "per",
  "perhaps",
  "placed",
  "please",
  "plus",
  "poorly",
  "possible",
  "possibly",
  "potentially",
  "pp",
  "predominantly",
  "present",
  "previously",
  "primarily",
  "probably",
  "promptly",
  "proud",
  "provides",
  "put",
  "q",
  "que",
  "quickly",
  "quite",
  "qv",
  "r",
  "ran",
  "rather",
  "rd",
  "re",
  "readily",
  "really",
  "recent",
  "recently",
  "ref",
  "refs",
  "regarding",
  "regardless",
  "regards",
  "related",
  "relatively",
  "research",
  "respectively",
  "resulted",
  "resulting",
  "results",
  "right",
  "run",
  "s",
  "said",
  "same",
  "saw",
  "say",
  "saying",
  "says",
  "sec",
  "section",
  "see",
  "seeing",
  "seem",
  "seemed",
  "seeming",
  "seems",
  "seen",
  "self",
  "selves",
  "sent",
  "seven",
  "several",
  "shall",
  "she",
  "shed",
  "she'll",
  "shes",
  "should",
  "shouldn't",
  "show",
  "showed",
  "shown",
  "showns",
  "shows",
  "significant",
  "significantly",
  "similar",
  "similarly",
  "since",
  "six",
  "slightly",
  "so",
  "some",
  "somebody",
  "somehow",
  "someone",
  "somethan",
  "something",
  "sometime",
  "sometimes",
  "somewhat",
  "somewhere",
  "soon",
  "sorry",
  "specifically",
  "specified",
  "specify",
  "specifying",
  "still",
  "stop",
  "strongly",
  "sub",
  "substantially",
  "successfully",
  "such",
  "sufficiently",
  "suggest",
  "sup",
  "sure	t",
  "take",
  "taken",
  "taking",
  "tell",
  "tends",
  "th",
  "than",
  "thank",
  "thanks",
  "thanx",
  "that",
  "that'll",
  "thats",
  "that've",
  "the",
  "their",
  "theirs",
  "them",
  "themselves",
  "then",
  "thence",
  "there",
  "thereafter",
  "thereby",
  "thered",
  "therefore",
  "therein",
  "there'll",
  "thereof",
  "therere",
  "theres",
  "thereto",
  "thereupon",
  "there've",
  "these",
  "they",
  "theyd",
  "they'll",
  "theyre",
  "they've",
  "think",
  "this",
  "those",
  "thou",
  "though",
  "thoughh",
  "thousand",
  "throug",
  "through",
  "throughout",
  "thru",
  "thus",
  "til",
  "tip",
  "to",
  "together",
  "too",
  "took",
  "toward",
  "towards",
  "tried",
  "tries",
  "truly",
  "try",
  "trying",
  "ts",
  "twice",
  "two",
  "u",
  "un",
  "under",
  "unfortunately",
  "unless",
  "unlike",
  "unlikely",
  "until",
  "unto",
  "up",
  "upon",
  "ups",
  "us",
  "use",
  "used",
  "useful",
  "usefully",
  "usefulness",
  "uses",
  "using",
  "usually",
  "v",
  "value",
  "various",
  "'ve",
  "very",
  "via",
  "viz",
  "vol",
  "vols",
  "vs",
  "w",
  "want",
  "wants",
  "was",
  "wasnt",
  "way",
  "we",
  "wed",
  "welcome",
  "we'll",
  "went",
  "were",
  "werent",
  "we've",
  "what",
  "whatever",
  "what'll",
  "whats",
  "when",
  "whence",
  "whenever",
  "where",
  "whereafter",
  "whereas",
  "whereby",
  "wherein",
  "wheres",
  "whereupon",
  "wherever",
  "whether",
  "which",
  "while",
  "whim",
  "whither",
  "who",
  "whod",
  "whoever",
  "whole",
  "who'll",
  "whom",
  "whomever",
  "whos",
  "whose",
  "why",
  "widely",
  "willing",
  "wish",
  "with",
  "within",
  "without",
  "wont",
  "words",
  "world",
  "would",
  "wouldnt",
  "www",
  "x",
  "y",
  "yes",
  "yet",
  "you",
  "youd",
  "you'll",
  "your",
  "youre",
  "yours",
  "yourself",
  "yourselves",
  "you've","z","zero"
            };

            List<string> word = new List<string>();
            foreach (var term in words)
            {
                for (int i = 0; i < stopwords.Length; i++)
                {
                    if (term == (stopwords[i]))
                    {
                        word.Add(term);
                    }

                }
            }

            foreach (var term in word)
            {
                words.Remove(term);

            }


            return words;
        }
        protected int getTermsFromDatabase(List<string> words)
        {
            // add words in form query
            //string words = "('score','slap')";
            List<termDocPos> termsNoOrder = new List<termDocPos>();
            string query = "";
            // List<string> words = new List<string>() { "score", "slap", "help" };
            query += "(";
            foreach (var word in words)
            {

                query += "'" + word.Trim().ToString() + "'" + ",";

            }
            query = query.Remove(query.Length - 1, 1);

            query += ")";


            try
            {
                string find = $"select [term],[DocAndPos] from invertedindexer where term IN " + query;

                SqlCommand comm = new SqlCommand(find, conn);
                //   comm.Parameters.AddWithValue("@words", words);
                conn.Open();
                dr = comm.ExecuteReader();

            }
            catch (Exception ex)
            {
                Console.WriteLine("there are Error");
            }
            if (dr.HasRows == false)
            {
                return 0;
            }
            while (dr.Read())
            {
                //terms.Add(new termDocPos() { term = dr["term"].ToString(), docAndPos = dr["DocAndPos"].ToString() });
                termsNoOrder.Add(new termDocPos() { term = dr["term"].ToString().Trim(), docAndPos = dr["DocAndPos"].ToString().Trim() });
            }
            conn.Close();

            // order depend on order of words
            foreach (var element in words)
            {
                foreach(var j in termsNoOrder)
                {

                    if (element == j.term)
                    {
                        terms.Add(new termDocPos() { term = j.term.ToString(), docAndPos = j.docAndPos.ToString() });

                    }

                }


                    }



            return words.Count;
        }

        protected void setDictionary()
        {
            //create dictionary to store words with docID and postions in each  doc
            result = new Dictionary<string, Dictionary<int, List<int>>>();
            string termText;
            string[] postLists;
            string[] doc;
            int docId;
            int docPos;

            //input (15,2),(15,3)
            foreach (var term in terms)
            {
                //delete () from string of document id and position
                term.docAndPos = term.docAndPos.Replace("(", "");
                term.docAndPos = term.docAndPos.Replace(")", "");
                //15,2|15,3
            }


            for (int i = 0; i < terms.Count; i++)
            {

                termText = terms[i].term.Trim().ToString();
                terms[i].docAndPos = terms[i].docAndPos.Remove(terms[i].docAndPos.Length - 1);
                postLists = terms[i].docAndPos.Split('|');

                foreach (var pList in postLists)
                {
                    doc = pList.Split(',');
                    docId = int.Parse(doc[0]);
                    docPos = int.Parse(doc[1]);

                    if (result.ContainsKey(termText))
                    {

                        if (result[termText].ContainsKey(docId))
                        {
                            result[termText][docId].Add(docPos);
                            result[termText][docId].Sort();
                            //  result[termText][docId].Re();
                        }
                        else
                        {
                            result[termText].Add(docId, new List<int>() { docPos });

                        }


                    }
                    else
                    {
                        var tempdic = new Dictionary<int, List<int>>();
                        tempdic.Add(docId, new List<int> { docPos });
                        result.Add(termText, tempdic);


                    }


                }


            }


        }


        protected void exactSearch()
        {
            //to calculate total frequency of words
            int wordsFreq ;
            bool testorder = true;
            int minnumber;
            orderWords = new List<int>();
            Wordsafterorder = new List<int>();
            interSections = new Dictionary<int, List<List<int>>>();
         
            orderWords = new List<int>();
            docsRank = new Dictionary<int, int>();
            int doc;
            int numberofIntersectionWords;

            foreach (var term in result)
            {
                foreach (var element in term.Value)
                {
                    if (interSections.ContainsKey(element.Key))
                    {
                        interSections[element.Key].Add(element.Value);

                    }
                    else
                    {
                        interSections.Add(element.Key, new List<List<int>>() { element.Value });
                    }
                }
            }


            foreach (var element in interSections)
            {
                numberofIntersectionWords = element.Value.Count;

                if (numberofIntersectionWords != numOfWords)
                {
                    // we don't have the Exact number of words in this element 

                    continue;

                }
            

               foreach(var lis in element.Value)
                {
                    minnumber= int.MaxValue;

                    foreach (var pos in lis)
                    { 
                        minnumber = Math.Min(pos,minnumber);
                     }


                    orderWords.Add(minnumber);
                    Wordsafterorder.Add(minnumber);
                }


                //check if list is sorted or not to check order
                Wordsafterorder.Sort();
                for (int i = 1; i < orderWords.Count; i++)
                {
                    if (orderWords[i]!=Wordsafterorder[i])
                    {
                        testorder = false;
                        break;
                    }
                
                
                }
                if (testorder == false)
                {

                    return;
                }



                //Ranking documentsPart
                //loop on each list to calculate the frequencey of words in doc
                wordsFreq = 0;
                foreach (var Lis in element.Value)
                {
                    wordsFreq += Lis.Count;

                }

                docsRank.Add(element.Key,wordsFreq);


            }
            // rank doc descending 
            foreach (var item in docsRank.OrderByDescending(key => key.Value))
            {
                continue;
            }

           
            //display results in gridview
            displayDocuments();



        }

        protected void multiKeywordSearch()
        {
            interSections = new Dictionary<int, List<List<int>>>();
            //getTermsFromDatabase(string[]query)
            //setDictionary();
            foreach (var term in result)
            {
                foreach (var element in term.Value)
                {
                    if (interSections.ContainsKey(element.Key))
                    {
                        interSections[element.Key].Add(element.Value);

                    }
                    else
                    {
                        interSections.Add(element.Key, new List<List<int>>() { element.Value });
                    }
                }
            }

            //Ranking documentsPart
            docsRank = new Dictionary<int, int>();
            int doc;
            int numberofIntersectionWords;
            foreach (var element in interSections)
            {
                numberofIntersectionWords = element.Value.Count;
                doc = element.Key;
                diff = new List<int>();
                if (numberofIntersectionWords == numOfWords)
                {

                    calcSmallestDistance(element.Value, 0, diff);
                    docsRank.Add(doc, mini);
                }
                else
                {
                    calcSmallestDistance(element.Value, 0, diff);
                    while (numberofIntersectionWords != numOfWords)
                    {
                        mini = mini + 100000;
                        numberofIntersectionWords++;
                    }
                    docsRank.Add(doc, mini);

                }

            }

            // to rank dictionary  
            foreach (KeyValuePair<int, int> item in docsRank.OrderBy(key => key.Value))
            {
                continue;
            }


            foreach (var document in docsRank)
            {
                print += document.Key + " and docId = " + document.Value + " , ";


            }

            // TextBox1.Text = print;

            displayDocuments();



        }
        public int calculateDiffrence(List<int> diff)
        {
            diff.Sort();
            int cnt = 0;
            for (int i = diff.Count - 1; i > 0; i--)
            {
                cnt += (diff[i] - diff[i - 1]);
            }
            return cnt;
        }



        void calcSmallestDistance(List<List<int>> pos, int index, List<int> diff)
        {
            if (index == pos.Count)
            {
                mini = Math.Min(mini, calculateDiffrence(diff));
                return;
            }
            foreach (var i in pos[index])
            {
                diff.Add(i);
                calcSmallestDistance(pos, index + 1, diff);
                diff.RemoveAt(diff.Count - 1);
            }
        }



     

      

        public void displayDocuments()
        {
            //  DataTable dt = new DataTable();
            dt = new DataTable();
            dt.Columns.Add("URLs", typeof(string));
            dt.Columns.Add("Proximity", typeof(string));
            foreach (var item in docsRank)
            {
               
                dt.Rows.Add(getDocURl(item.Key).ToString(), item.Value.ToString());
             

            }

            GridView1.DataSource = dt.DefaultView;
            GridView1.DataBind();

        }
        protected string getDocURl(int docIndex)
        {
            string index="";
            try
            {
                string find = "select URL from Websites where Id =" +"'"+docIndex.ToString()+"'";

                SqlCommand comm = new SqlCommand(find, conn);
                conn.Open();
                dr = comm.ExecuteReader();
              
            }catch (Exception ex)
            {
                Console.WriteLine("there are Error");
            }

            while (dr.Read())
            {
                index=dr["URL"].ToString();


            }

            
  
            conn.Close();
            return index;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
           
        }
    }
}
