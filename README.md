# miniSearchEngine

miniSearchEngine is a simple search engine project that extracts web pages from the BBC website, converts them into an inverted index format, and enables users to search for relevant articles. The project is divided into three main tasks: web scraping, database conversion, and searching.

## Prerequisites

- C# programming language
- HtmlAgilityPack library
- PorterStemmerAlgorithm for stemming
- Microsoft SQL Server for the database

## Task 1: Web Scraping

The web scraping task involves extracting web pages from the BBC website using the HtmlAgilityPack library in C#. The HtmlAgilityPack library provides a convenient way to parse HTML documents and extract specific elements or data. In this task, the project utilizes web scraping to gather approximately 200 pages from the BBC website.

## Task 2: Database Conversion

The database conversion task focuses on processing the scraped web pages and storing them in a Microsoft SQL Server database as an inverted index format. The inverted index allows for efficient retrieval of documents based on search queries. This task involves applying text processing techniques, such as tokenization, stemming (using the PorterStemmerAlgorithm), and creating an inverted index structure in the database.

## Task 3: Searching

The searching task is the final and main functionality of the miniSearchEngine project. Users can input search queries, and the system retrieves relevant articles based on the inverted index. The search query goes through the same text processing techniques used in the database conversion task. The system then searches the inverted index in the database to retrieve the most relevant articles based on the user's query.

## Usage

1. Clone the repository:
               
                git clone https://github.com/marwankhaled1/miniSearchEngine.git
                cd miniSearchEngine
                
2. Set up the required dependencies, including C# and Microsoft SQL Server.

3. Execute the web scraping task to gather web pages from the BBC website.

4. Run the database conversion task to process the scraped pages and create the inverted index in the Microsoft SQL Server database.

5. Start the search functionality to allow users to input search queries and retrieve relevant articles.

6. Modify and optimize the code as needed for further improvements or customization.

## Contributing

Contributions to this project are welcome! If you have any ideas, suggestions, or bug fixes, feel free to open an issue or submit a pull request. Your contributions can help enhance the functionality and performance of the miniSearchEngine.



