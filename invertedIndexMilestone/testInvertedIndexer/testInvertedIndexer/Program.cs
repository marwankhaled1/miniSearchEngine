using HtmlAgilityPack;
using Searcharoo.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace testInvertedIndexer
{

    class termDoc
    {
        public string term;
        public int docId;
        public int pos;
    }    //hold docId and term pos in it 
    class postinglist
    {
        public int docId;
        public int pos;

    };


    class Program
    {
        static string textpath = "";
        static SqlCommand sc;
        static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Marwan\source\repos\crawlerTaskIR\crawlerTaskIR\simpleDatabase.mdf; Integrated Security = True";
        static SqlConnection conn = new SqlConnection(connString);
        static List<termDoc> termAndDocumentId;
        static PorterStemmer stemmer = new PorterStemmer();
        static List<postinglist> tempPostions;
        static Dictionary<int, int> freqINDoc;
        [Obsolete]
        static void Main(string[] args)
        {

            // removedatafromDatabase();
             //return;

            Dictionary<int, string> documents = new Dictionary<int, string>();
            Dictionary<string, List<postinglist>> invertedindexer = new Dictionary<string, List<postinglist>>();

            int docId = 1;
            termAndDocumentId = new List<termDoc>();

            // Case folding and remove (number, non English characters) in getDocuments() before save into documents
            // && tokenizing and remove punctuation in converttoTermDocId()

            //insert extracted text in documents 
            getDocuments(documents);

            //convert documents into term and docId and store in  termAndDocumentId list
            foreach (var document in documents)
            {

                converttoTermDocId(document, docId);
                docId++;
            }

            //add terms to term-doc table in database
            addtermDoctoDB();



            //stemming and add to inverted indexer 
            addtoinvertedIndexer(invertedindexer);


            // sort inverted indexer
            invertedindexer = invertedindexer.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);


            //remove stop words
            removeStopWords(invertedindexer);



            // add inverted indexer to database
            addInvertedIndexertoDB(invertedindexer);


        }

        public static void addtermDoctoDB()
        {
            for (int i = 1; i < termAndDocumentId.Count; i++)
            {
                /*string path = @"E:/inverted" + '/' + "termdoc" + ".txt";

                FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
                StreamWriter file = new StreamWriter(fileStream);
*/
                try
                {
                    
                   
                   
                   
                    conn.Open();
                    sc = new SqlCommand("insert into termDocId(term,docID,pos) values(@term,@docID,@pos)", conn);
                    sc.Parameters.AddWithValue("term", termAndDocumentId.ElementAt(i).term.Trim());
                  //  file.WriteLine("term: " + termAndDocumentId.ElementAt(i).term.Trim());


                    sc.Parameters.AddWithValue("docID", termAndDocumentId.ElementAt(i).docId);
                   // file.WriteLine(("docID: " + termAndDocumentId.ElementAt(i).docId));


                    sc.Parameters.AddWithValue("pos", termAndDocumentId.ElementAt(i).pos);
                 //   file.WriteLine(("pos: " + termAndDocumentId.ElementAt(i).pos));
                    sc.ExecuteNonQuery();
                    sc.Dispose();
                    conn.Close();
                   

                }
                catch (Exception ex)
                {
                   // Console.WriteLine(ex.ToString());
                    termAndDocumentId.RemoveAt(i);
                    conn.Close();
                  /*  file.Close();
                    fileStream.Close();*/
                    continue;
                }
                /*file.Close();
                fileStream.Close();*/

            }
        }



        static public string fetchfile(int id)
        {

            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            sc = new SqlCommand("select * from Websites where id=@id", conn);
            sc.Parameters.AddWithValue("id", id);
            SqlDataReader reader = sc.ExecuteReader();

            while (reader.Read())
            {
                textpath = reader["path"].ToString();
            }

            sc.Dispose();
            conn.Close();
            return textpath;

        }


        static public void getDocuments(Dictionary<int, string> documents)
        {

            for (int count = 1; count < 1600; count++)
            {
                String text = "";

                // Access database to read files and  fetch file 
                string fileurl = fetchfile(count);
                var doc = new HtmlDocument();
                doc.Load(fileurl);

                // Remove script & style nodes
                doc.DocumentNode.Descendants().Where(n => n.Name == "script" || n.Name == "style").ToList().ForEach(n => n.Remove());

                var nodes = doc.DocumentNode.SelectNodes("//p");
                if (nodes == null)
                {
                    continue;
                }

                foreach (HtmlNode node in nodes)
                {
                    // skip empty tags
                    if (node == null)
                    {
                        continue;
                    }

                    if (node.InnerText.Contains(';'))
                    {
                        continue;
                    }
                     
                    //remove numbers lines
                    if (Regex.IsMatch(node.InnerText, @"^\d+$"))
                    {
                        continue;
                    }
                     
                    // Case folding
                    text += node.InnerText.Trim().ToLower();
                    text += "\n";
                }

                // Remove non English character and numbers
                text = Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);
             //   text = text.Trim();
                //replace multiple spaces with one space
                text = text.Replace("\n", " ");
                //remove numbers from lines
                text = Regex.Replace(text, @"[\d-]", string.Empty);
                documents.Add(count, text);
            }
    }




        static public string[] tokenizing(String text)
        {
            string[] tokens = text.Split(new[] { ',', ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
            return tokens;
        }




        static public string[] removePunctuation(string[] tokens)
        {
            string[] noPuncTockens = new string[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                noPuncTockens[i] = new string(tokens[i].Where(c => !char.IsPunctuation(c)).ToArray());
            }
            return noPuncTockens;
        }




        static public void converttoTermDocId(KeyValuePair<int, string> document, int docID)
        {
            // case folding before save text into dictionary  in converttoTermDocId()
            // split documents into tokens
            string[] tokens = tokenizing(document.Value);


            // remove punctuation  
            string[] afterPunToken = removePunctuation(tokens);
            int position = 1;

            //add 
            foreach (var term in afterPunToken)
            {
                if (position == 101)
                {
                    break;
                
                }
                
                    termDoc temp = new termDoc();
                    temp.term = term;
                    temp.docId = docID;
                    temp.pos = position;
                    termAndDocumentId.Add(temp);
                    position++;
                
            
            
            }
        }




        static public void removedatafromDatabase()
        {   conn.Open();
       // sc = new SqlCommand("DELETE FROM termDocId", conn);
        sc = new SqlCommand("DELETE FROM invertedindexer", conn);
            sc.ExecuteNonQuery();
            sc.Dispose();
            conn.Close();

        }

        static public void addtoinvertedIndexer(Dictionary<string, List<postinglist>> invertedindexer)
        {
            string tempstring = "";
            foreach (var input in termAndDocumentId)
            {
                postinglist tempPost = new postinglist();
                tempstring = stemmer.StemWord(input.term);


                tempPost.docId = input.docId;
                tempPost.pos = input.pos;

                if (invertedindexer.ContainsKey(tempstring))
                {


                    //   invertedindexer[tempstring].Append<postinglist>(tempPost);
                    invertedindexer[tempstring].Add(tempPost);
                    continue;
                }

                List<postinglist> templist = new List<postinglist> { tempPost };
                invertedindexer.Add(tempstring, templist);
            }

            invertedindexer.Remove("");
        }




        static public void addInvertedIndexertoDB(Dictionary<string, List<postinglist>> invertedindexer)
        {
           string path = @"E:/inverted" + '/' + "invertedindex" + ".txt";

           

           

            // add inverted indexer to database  
            for (int i = 1; i < invertedindexer.Count; i++)
            {
                freqINDoc = new Dictionary<int, int>();
                int termDocFrequency; //dictionary to store doc and position
               /* FileStream fileStream = File.Open(path, FileMode.OpenOrCreate);
                StreamWriter file = new StreamWriter(fileStream);*/
                string pos = "";
                string posINDoc = "";
                
                try
                {
                    
                    conn.Open();
                    sc = new SqlCommand("insert into  invertedindexer(term,freq,DocAndPos,freqINDoc) values(@term,@freq,@DocAndPos,@freqINDoc)", conn);
                    sc.Parameters.AddWithValue("term", invertedindexer.ElementAt(i).Key);
                    sc.Parameters.AddWithValue("freq", invertedindexer.ElementAt(i).Value.Count);
                   
                    tempPostions = invertedindexer.ElementAt(i).Value;
                    foreach (var j in tempPostions)
                    {

                        pos += "(" + j.docId + "," + j.pos + ")"+"|";

                        // put docid and frequency in each doc in 

                        if (freqINDoc.ContainsKey(j.docId))
                        {
                            termDocFrequency = freqINDoc[j.docId];
                            termDocFrequency = termDocFrequency +1;
                            freqINDoc[j.docId] = termDocFrequency;
                        }
                        else
                        {
                           
                            freqINDoc.Add(j.docId, 1);
                        }
                    }

                    //put frequency in each document in frqINDoc column

                    foreach (var element in freqINDoc)
                    {
                        posINDoc += "(" + element.Key.ToString() + "," + element.Value.ToString() + ")"+"|";

                    }


                    sc.Parameters.AddWithValue("DocAndPos", pos);
                    sc.Parameters.AddWithValue("freqINDoc", posINDoc);

                   /* file.WriteLine("term: " + invertedindexer.ElementAt(i).Key);
                    file.WriteLine("freq: " + invertedindexer.ElementAt(i).Value.Count);
                    file.WriteLine("docANdpos: " + pos);*/
                    sc.ExecuteNonQuery();
                    sc.Dispose();
                    conn.Close();
                  /*  file.Close();
                    fileStream.Close();*/

                }
                catch (Exception ex)
                {
                  //  Console.WriteLine(ex.ToString());
                    invertedindexer.Remove(invertedindexer.ElementAt(i).Key);
                    conn.Close();
                   /* file.Close();
                    fileStream.Close();*/
                    continue;
                }

            }
          

        }


    static public void removeStopWords(Dictionary<string, List<postinglist>> invertedindexer)
        {
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
            string stemStopWord = "";

            for (int i = 0; i < stopwords.Length; i++)
            {
                if (invertedindexer.ContainsKey(stopwords[i]))
                {
                    stemStopWord = stemmer.StemWord(stopwords[i]);
                    invertedindexer.Remove(stemStopWord);
                }

            }


        }
    }
}