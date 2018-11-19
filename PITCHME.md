# Elasticsearch & .Net

![image](https://www.elastic.co/static/images/elastic-logo-200.png)

---

<!-- page_number: true -->

# Hi

email: robertlyson@gmail.com

twitter: @robertlyson

---

# Outline

- [Preparing Environment](#preparing-environment)
- [But, why elasticsearch?](#but-why-elasticsearch) 
- [Let's meet elasticsearch](#lets-meet-elasticsearch)
- [What is relevant search?](#what-is-relevant-search)
- [Exercise 1 - relevant search](#exercise1)
- [Exercise 2 - coffeshop finder](#exercise2)
- [Exercise 3 - percolate query](#exercise3)
- [I want more](#i-want-more)

---

# Few questions to you

- are you familiar with .net?
- have you ever played with elasticsearch before?
- are you using elasticsearch in your projects?

---

# [Preparing Environment](#preparing-environment)

Two ways:

- local download/install

or

- docker compose

---

## java 1.8.x

http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html

![image](https://user-images.githubusercontent.com/2392583/40742808-ec1ff66e-644f-11e8-8d05-7414176e63f4.png)

---

## elasticsearch 6.2.4

download https://www.elastic.co/downloads/past-releases/elasticsearch-6-2-4
follow installation steps https://www.elastic.co/downloads/elasticsearch

After you downloaded and run elasticsearch(ES) instance, `http://localhost:9200` in your browser should return
```
{
  "name" : "awTmG2s",
  "cluster_name" : "elasticsearch",
  "cluster_uuid" : "5FygmZx4TuqvPJP0io_fNw",
  "version" : {
    "number" : "6.2.4",
    ...
  },
  "tagline" : "You Know, for Search"
}
```

---

## kibana 6.2.4 1/2

download https://www.elastic.co/downloads/past-releases/kibana-6-2-4
and also, follow instalation steps :) https://www.elastic.co/downloads/kibana

---

## kibana 6.2.4 2/2

Maku sure all is good by going to `http://localhost:5601` in your fav. browser. You should see something like

![capture](https://user-images.githubusercontent.com/2392583/39409275-7207374e-4be4-11e8-9c83-5306aa0ebc66.PNG)

---

## or docker

https://gist.github.com/robertlyson/71e1d096597ed446836de9b084b3c913

---

## dotnet core 2.0+

https://www.microsoft.com/net/download/windows

Run `dotnet --version`, will print 
```
Rob@DESKTOP-CFOA2QH C:\somedirectory
$ dotnet --version
2.1.100
```
or something very similar.

---

We are done!

---

## [But, why elasticsearch?](#but-why-elasticsearch)

---

# my story

![image](https://media.giphy.com/media/3o7TKDvZhx5J7MBnc4/giphy.gif)

---

# 1/6

Requirement was simple, allow users to search for branches* by name. 
So we did it!

*this is how banks like to call theirs locations(ATM, department, office)

---

# 2/6

```sql
SELECT TOP 1 [Id]
      ,[UniqueCode]
      ,[LocationName]
      ,[LocationShortName]
      ,[LocationNumber]
      ,[LocationAdress1]
      ,[LocationCounty]
      ,[LocationZipCode]
      ,[LocationContact]
      ,[Latitude]
      ,[Longitude]
      ,[IsAtm]
  FROM [DevWarsztatyRelevantSearch].[dbo].[Branch]
  where LOWER(LocationName) like '%morgan%'
```

---

# 3/6

Worked very well.

![image](https://user-images.githubusercontent.com/2392583/40502596-bb3dd750-5f8b-11e8-96e7-eaea5ac99c0a.png)

---

# 4/6

Unfortunately, few days later after we deployed this feature to users, it turned out something is wrong. Someone typed `Morgan` in search box and got as a result:

![image](https://user-images.githubusercontent.com/2392583/40502676-fe739c4e-5f8b-11e8-8bb9-ece82b83f7e8.png)

--- 

# 5/6

```sql
SELECT TOP 1 [Id]
      ,[UniqueCode]
      ,[LocationName]
      ,[LocationShortName]
      ,[LocationNumber]
      ,[LocationAdress1]
      ,[LocationCounty]
      ,[LocationZipCode]
      ,[LocationContact]
      ,[Latitude]
      ,[Longitude]
      ,[IsAtm]
  FROM [DevWarsztatyRelevantSearch].[dbo].[Branch]
  where LOWER(LocationName) like '%morgan%'
  or LOWER(LocationContact) like '%morgan%'
```

---

# 6/6

```sql
or LOWER(LocationContact) like '%morgan%'
```

![image](https://media.giphy.com/media/3osxYoT363g5uv72bm/giphy.gif)'

We will try to fix it later on .. I promise.

---

## [Let's meet elasticsearch](#lets-meet-elasticsearch)

---

Some facts about elasticsearch:

- NoSQL database / full text search engine
- based on lucene
- simplify very complex lucene API with REST API
- distributed by nature
- OSS
- who is using it: wikipedia, spotify, netflix etc

---

# use cases 1/2

So, for what we can use it?

- realtime search
- very flexible/configurable full text search
- faceted search: https://bit.ly/2laAWr9
- reporting
- alerting
- monitoring: apps, servers, resources
- matching systems
- autocompletion

---

# use cases 2/2

today:
- very flexible/configurable search
- alerting

--- 

Made by Elastic company.
![image](https://user-images.githubusercontent.com/2392583/41193412-859f3972-6c0b-11e8-8491-ef0b558c7685.png)
![image](https://user-images.githubusercontent.com/2392583/41193419-9f4b360a-6c0b-11e8-8b29-21baf688eec0.png)

---


## Document based

```
{
	"email": "john@smith.com",
	"first_name": "John",
	"last_name": "Smith",
	"info": {
		"bio": "Eco-warrior and defender of the weak",
		"age": 25,
		"interests": ["dolphins", "whales"]
	},
	"join_date": "2014/05/01"
}
```

Document -> indexing -> index content / store document. 
We will talk more about this process in the relevant search section.

---

## How to communicate with elasticsearch

---

# 1/3

curl from cmd 
```
curl -XGET http://localhost:9200
```

---

# 2/3

kibana and dev tools

![image](https://user-images.githubusercontent.com/2392583/40768166-cc7e8cca-64b4-11e8-9c48-1751dc5d49a8.png)
This is a simple query returning 10 documents from all ES indices.

---

# 3/3

elasticsearch clients - in our case that will be NEST, but there are others:
Java, PHP, Python, Ruby, Erlang, Haskell ..


---

## NEST

# 1/3

First things first - client init:
```csharp
var uri = new Uri("http://localhost:9200");
var settings = new ConnectionSettings(uri);
settings.DefaultIndex(IndexName); // so we don't need to specify index with each call on elasticClient
settings.EnableDebugMode(); // because we want to see request/response body in IResponse from elasticClient
var client = new ElasticClient(settings);
```

---

# 2/3

With NEST we have two options to write requests

Fluent API:
```csharp
var searchResponse = client.Search<Project>(s => s
    .Query(q => q
        .MatchAll()
    )
);
```

---

# 3/3

Obiect initializer syntax:
```
var searchRequest = new SearchRequest<Project>
{
    Query = new MatchAllQuery()
};
```

---

## Indexing documents

Let's try it in Kibana.

---

# 1/4

```
PUT /megacorp/employee/1
{
    "first_name" : "John",
    "last_name" :  "Smith",
    "age" :        25,
    "about" :      "I love to go rock climbing",
    "interests": [ "sports", "music" ]
}
```

`megacorp` - index name. Something like database in sql world(more or less)
`employee` - type name, starting from ES 6.x, one per index. Something like table, not true in 100% e.g. terms were visible between types

---

# 2/4

In NEST you can index document by doing:

```csharp
elasticClient.Index(
    new Employee
    {
        Id = 1,
        FirstName = "John",
        LastName = "Smith",
        Age = 25,
        About = "I love to go rock climbing",
        Interests = new[] {"sports", "music"}
    }, d => d.Index("megacorp").Type("employee"));
```

---

# 3/4

Index mapping

```GET megacorp/_mapping```

---

# 4/4

bulk operations

```
POST _bulk
{ "index": { "_index": "megacorp", "_type": "employee", "_id": "2" } }
{"first_name": "Jane","last_name": "Smith", "age": 32,"about": "I like to collect rock albums","interests": [ "music" ]}
{ "index": { "_index": "megacorp", "_type": "employee", "_id": "3" } }
{"first_name": "Douglas","last_name": "Fir","age": 35,"about": "I like to build cabinets","interests": [ "forestry" ]}
```

---

## Retrieving a document

---

# 1/2

```
GET /megacorp/employee/1
```

---

# 2/2

With NEST:

```
elasticClient.Get<Employee>(1);
```
or
```
await elasticClient.GetAsync<Employee>(1);
```

---

## Search lite

---

# 1/2


```
GET /megacorp/employee/_search
```

---

# 2/2

```
GET /megacorp/employee/_search?q=last_name:Smith
```

Handy for simple queries, command line scenarios.

---

# Before we will move on please start cloning this repo

```git clone https://github.com/robertlyson/RelevantSearch.git```

---

## [What is relevant search?](#what-is-relevant-search)

---

Relevance search is ... ?

---

# How elasticsearch is helping?

![image](https://user-images.githubusercontent.com/2392583/41398262-0f9c1d6c-6fb7-11e8-9c9a-02e0fb4020ac.png)

---

# Let's see what we want to improve

---

![image](https://user-images.githubusercontent.com/2392583/41219086-8f62d56a-6d5d-11e8-82d4-7944df7d4169.png)

---

![image](https://user-images.githubusercontent.com/2392583/41368390-167e13bc-6f42-11e8-9ecf-70bfdf20516d.png)

---

![image](https://user-images.githubusercontent.com/2392583/41368312-e169781a-6f41-11e8-9b49-c03d6037c702.png)

---

# My issues with nuget search

- for query elasticsearch official client is listed on 4th position and NEST is not even there
- can't handle typos, no results for query elasti**sc**earch

---

# Fix it with elasticsearch

https://gist.github.com/robertlyson/79615e606a304a0416da83c4769c1153

---

# Part 1 of 20

<!-- $size: A2 -->

```json
DELETE nuget 

PUT nuget
{
    "settings" : {
        "index" : {
            "number_of_shards" : 1
        }
    }
}

POST _bulk
{"index":{"_index":"nuget","_type":"package","_id":"ElasticSearch"}}
{"id":"ElasticSearch","description":"Elasticsearch is a flexible and powerful open source, distributed, real-time search and analytics engine.","downloadCount":6669}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.Connection.Thrift"}}
{"id":"Elasticsearch.Net.Connection.Thrift","description":"An IConnection implementation that utilizes Apache Thrift to talk with elasticsearch","downloadCount":12747}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.Connection.HttpClient"}}
{"id":"Elasticsearch.Net.Connection.HttpClient","description":"An IConnection implementation that uses System.Net.Http.HttpClient to talk with elasticsearch","downloadCount":8917}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net"}}
{"id":"Elasticsearch.Net","description":"Elasticsearch.Net","downloadCount":3640838}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.JsonNET"}}
{"id":"Elasticsearch.Net.JsonNET","description":"This package is only useful if you use the low level client ONLY and do not use NEST but would like to use JSON.NET as your serializer","downloadCount":17511}
{"index":{"_index":"nuget","_type":"package","_id":"ElasticSearch.Diagnostics"}}
{"id":"ElasticSearch.Diagnostics","description":"TraceListener for ElasticSearch and kibana","downloadCount":7521}
{"index":{"_index":"nuget","_type":"package","_id":"Glimpse.ElasticSearch"}}
{"id":"Glimpse.ElasticSearch","description":"A simple Glimpse plugin for ElasticSearch. It shows queries and response times.","downloadCount":5685}
{"index":{"_index":"nuget","_type":"package","_id":"NEST"}}
{"id":"NEST","description":"Strongly typed interface to Elasticsearch. Fluent and classic object initializer mappings of requests and responses. Uses and exposes Elasticsearch.Net","downloadCount":2860924}
{"index":{"_index":"nuget","_type":"package","_id":"MiniProfiler.Elasticsearch"}}
{"id":"MiniProfiler.Elasticsearch","description":"Elasticsearch.Net and NEST client for logging to MiniProfiler.","downloadCount":1132}
{"index":{"_index":"nuget","_type":"package","_id":"NEST.Watcher"}}
{"id":"NEST.Watcher","description":"Watcher is a plugin for Elasticsearch that provides alerting and notifications based on changes in your data. This NuGet package extends NEST, allowing you to interface with the Watcher plugin.","downloadCount":1576}
{"index":{"_index":"nuget","_type":"package","_id":"AutoMapper"}}
{"id":"AutoMapper","description":"A convention-based object-object mapper.","downloadCount":19278096}
{"index":{"_index":"nuget","_type":"package","_id":"Newtonsoft.Json"}}
{"id":"Newtonsoft.Json","description":"A convention-based object-object mapper.","downloadCount":126514991}
```

---

# Part 2 of 20

<!-- $size: 16:9 -->

Is data in?

```json
GET nuget/_search
{
  "query": {
    "match_all": {}
  }
}
```

---

# Part 3 of 20

Can we go home? Are we better than nuget search already?

```json
GET /nuget/_search
{
  "query": {
    "multi_match": {
      "query": "elasticsearch",
      "fields": ["id","description"]
    }
  }
}
```

---

# Part 4 of 20

Not really
1. first result is not official client .. not quite popular, only 6669 downloads
2. elasticsearch.net is not even there!
3. NEST is on the list .. that's good

---

# Part 5 of 20

So, why elasticsearch.net is not listed?

```
GET _analyze
{
  "analyzer" : "standard",
  "text" : "Elasticsearch.Net"
}
```

---

# Part 6 of 20

Custom analyzer to the rescue.

<!-- $size: A2 -->

```
DELETE nuget

PUT nuget
{
    "settings": {
        "index": {
            "number_of_shards" : 1,
            "analysis": {
                "char_filter": {
                    "my_pattern": {
                        "type": "pattern_replace",
                        "pattern": "\\.",
                        "replacement": " "
                    }
                },
                "analyzer": {
                    "my_analyzer": {
                         "char_filter": [
                            "my_pattern"
                        ],
                        "tokenizer": "standard",
                        "filter": ["lowercase"]
                    }
                }
            }
        }
    },
    "mappings": {
        "package": {
            "properties": {
                "id": {
                    "type": "text",
                    "analyzer": "my_analyzer"
                },
                "description": {
                    "type": "text",
                    "analyzer": "my_analyzer"
                }
            }
        }
    }
}
```

---

# Part 7 of 20

<!-- $size: 16:9 -->

Hopefully our analyzer knows how to deal with dots.

```
GET nuget/_analyze
{
  "analyzer" : "my_analyzer",
  "text" : "Elasticsearch.Net"
}
```

---

# Part 8 of 20

<!-- $size: A3 -->

And idexing one more time ..

```json
POST _bulk
{"index":{"_index":"nuget","_type":"package","_id":"ElasticSearch"}}
{"id":"ElasticSearch","description":"Elasticsearch is a flexible and powerful open source, distributed, real-time search and analytics engine.","downloadCount":6669}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.Connection.Thrift"}}
{"id":"Elasticsearch.Net.Connection.Thrift","description":"An IConnection implementation that utilizes Apache Thrift to talk with elasticsearch","downloadCount":12747}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.Connection.HttpClient"}}
{"id":"Elasticsearch.Net.Connection.HttpClient","description":"An IConnection implementation that uses System.Net.Http.HttpClient to talk with elasticsearch","downloadCount":8917}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net"}}
{"id":"Elasticsearch.Net","description":"Elasticsearch.Net","downloadCount":3640838}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.JsonNET"}}
{"id":"Elasticsearch.Net.JsonNET","description":"This package is only useful if you use the low level client ONLY and do not use NEST but would like to use JSON.NET as your serializer","downloadCount":17511}
{"index":{"_index":"nuget","_type":"package","_id":"ElasticSearch.Diagnostics"}}
{"id":"ElasticSearch.Diagnostics","description":"TraceListener for ElasticSearch and kibana","downloadCount":7521}
{"index":{"_index":"nuget","_type":"package","_id":"Glimpse.ElasticSearch"}}
{"id":"Glimpse.ElasticSearch","description":"A simple Glimpse plugin for ElasticSearch. It shows queries and response times.","downloadCount":5685}
{"index":{"_index":"nuget","_type":"package","_id":"NEST"}}
{"id":"NEST","description":"Strongly typed interface to Elasticsearch. Fluent and classic object initializer mappings of requests and responses. Uses and exposes Elasticsearch.Net","downloadCount":2860924}
{"index":{"_index":"nuget","_type":"package","_id":"MiniProfiler.Elasticsearch"}}
{"id":"MiniProfiler.Elasticsearch","description":"Elasticsearch.Net and NEST client for logging to MiniProfiler.","downloadCount":1132}
{"index":{"_index":"nuget","_type":"package","_id":"NEST.Watcher"}}
{"id":"NEST.Watcher","description":"Watcher is a plugin for Elasticsearch that provides alerting and notifications based on changes in your data. This NuGet package extends NEST, allowing you to interface with the Watcher plugin.","downloadCount":1576}
{"index":{"_index":"nuget","_type":"package","_id":"AutoMapper"}}
{"id":"AutoMapper","description":"A convention-based object-object mapper.","downloadCount":19278096}
{"index":{"_index":"nuget","_type":"package","_id":"Newtonsoft.Json"}}
{"id":"Newtonsoft.Json","description":"A convention-based object-object mapper.","downloadCount":126514991}
```

---

# Part 9 of 20

Is it something better?

```
GET /nuget/_search
{
  "query": {
    "multi_match": {
      "query": "elasticsearch",
      "fields": ["id","description"]
    }
  }
}
```

---

# Part 10 of 20

Not much better, only NEST is a little bit higher.

What about boosting id field as it seems more important?

```
GET /nuget/_search
{
  "query": {
    "multi_match": {
      "query": "elasticsearch",
      "fields": ["id^2","description"]
    }
  }
}
```

---

# Part 11 of 20

Promising, let's consider download count as a boost.

```
GET nuget/_search
{
  "query": {
    "function_score": {
      "query": {
        "multi_match": {
          "query": "elasticsearch",
          "fields": [
            "id^2",
            "description"
          ]
        }
      },
      "functions": [
        {
          "field_value_factor": {
            "field": "downloadCount",
            "factor": 0.00001
          }
        }
      ]
    }
  }
}
```

---

# Part 12 of 20

Uff, beautiful. How it's working for NEST query?

```json
GET nuget/_search
{
  "query": {
    "function_score": {
      "query": {
        "multi_match": {
          "query": "nest",
          "fields": [
            "id^2",
            "description"
          ]
        }
      },
      "functions": [
        {
          "field_value_factor": {
            "field": "downloadCount",
            "factor": 0.00001
          }
        }
      ]
    }
  }
}
```

---

# Part 13 of 20

No mention of elasticsearch.net. 
So maybe let's create synonym for NEST ..

---

# Part 14 of 20

<!-- $size: A2 -->

```
DELETE nuget

PUT nuget
{
    "settings": {
        "index": {
            "number_of_shards" : 1,
            "analysis": {
                "char_filter": {
                    "my_pattern": {
                        "type": "pattern_replace",
                        "pattern": "\\.",
                        "replacement": " "
                    }
                },
                "analyzer": {
                    "my_analyzer": {
                         "char_filter": [
                            "my_pattern"
                        ],
                        "tokenizer": "standard",
                        "filter": ["lowercase","synonym"]
                    }
                },
                "filter" : {
                  "synonym": {
                    "type": "synonym",
                    "synonyms": [
                      "nest => elasticsearch"
                      ]
                  }
                }
            }
        }
    },
    "mappings": {
        "package": {
            "properties": {
                "id": {
                    "type": "text",
                    "analyzer": "my_analyzer"
                },
                "description": {
                    "type": "text",
                    "analyzer": "my_analyzer"
                }
            }
        }
    }
}
```

---

# Part 15 of 20

<!-- $size: 16:9 -->

```
GET nuget/_analyze
{
  "analyzer" : "my_analyzer",
  "text" : "nest"
}
```

---

# Part 16 of 20

<!-- $size: A2 -->

Works like a charm, let's index data one more time ..

```
POST _bulk
{"index":{"_index":"nuget","_type":"package","_id":"ElasticSearch"}}
{"id":"ElasticSearch","description":"Elasticsearch is a flexible and powerful open source, distributed, real-time search and analytics engine.","downloadCount":6669}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.Connection.Thrift"}}
{"id":"Elasticsearch.Net.Connection.Thrift","description":"An IConnection implementation that utilizes Apache Thrift to talk with elasticsearch","downloadCount":12747}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.Connection.HttpClient"}}
{"id":"Elasticsearch.Net.Connection.HttpClient","description":"An IConnection implementation that uses System.Net.Http.HttpClient to talk with elasticsearch","downloadCount":8917}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net"}}
{"id":"Elasticsearch.Net","description":"Elasticsearch.Net","downloadCount":3640838}
{"index":{"_index":"nuget","_type":"package","_id":"Elasticsearch.Net.JsonNET"}}
{"id":"Elasticsearch.Net.JsonNET","description":"This package is only useful if you use the low level client ONLY and do not use NEST but would like to use JSON.NET as your serializer","downloadCount":17511}
{"index":{"_index":"nuget","_type":"package","_id":"ElasticSearch.Diagnostics"}}
{"id":"ElasticSearch.Diagnostics","description":"TraceListener for ElasticSearch and kibana","downloadCount":7521}
{"index":{"_index":"nuget","_type":"package","_id":"Glimpse.ElasticSearch"}}
{"id":"Glimpse.ElasticSearch","description":"A simple Glimpse plugin for ElasticSearch. It shows queries and response times.","downloadCount":5685}
{"index":{"_index":"nuget","_type":"package","_id":"NEST"}}
{"id":"NEST","description":"Strongly typed interface to Elasticsearch. Fluent and classic object initializer mappings of requests and responses. Uses and exposes Elasticsearch.Net","downloadCount":2860924}
{"index":{"_index":"nuget","_type":"package","_id":"MiniProfiler.Elasticsearch"}}
{"id":"MiniProfiler.Elasticsearch","description":"Elasticsearch.Net and NEST client for logging to MiniProfiler.","downloadCount":1132}
{"index":{"_index":"nuget","_type":"package","_id":"NEST.Watcher"}}
{"id":"NEST.Watcher","description":"Watcher is a plugin for Elasticsearch that provides alerting and notifications based on changes in your data. This NuGet package extends NEST, allowing you to interface with the Watcher plugin.","downloadCount":1576}
{"index":{"_index":"nuget","_type":"package","_id":"AutoMapper"}}
{"id":"AutoMapper","description":"A convention-based object-object mapper.","downloadCount":19278096}
{"index":{"_index":"nuget","_type":"package","_id":"Newtonsoft.Json"}}
{"id":"Newtonsoft.Json","description":"A convention-based object-object mapper.","downloadCount":126514991}
```

---

# Part 17 of 20

<!-- $size: A2 -->

Do we have it?

```
GET nuget/_search
{
  "query": {
    "function_score": {
      "query": {
        "multi_match": {
          "query": "nest",
          "fields": [
            "id^2",
            "description"
          ]
        }
      },
      "functions": [
        {
          "field_value_factor": {
            "field": "downloadCount",
            "factor": 0.00001
          }
        }
      ]
    }
  }
}
```

---

# Part 18 of 20

Hmm, nest is second?
Maybe function boost is doing too much pressure on download count?

```
GET nuget/_search
{
  "query": {
    "function_score": {
      "query": {
        "multi_match": {
          "query": "nest",
          "fields": [
            "id^2",
            "description"
          ]
        }
      },
      "max_boost": 10, 
      "functions": [
        {
          "field_value_factor": {
            "field": "downloadCount",
            "factor": 0.00001
          }
        }
      ]
    }
  }
}
```

---

# Part 19 of 20

Are we fine with typos?

```
GET nuget/_search
{
  "query": {
    "function_score": {
      "query": {
        "multi_match": {
          "query": "elastiscearch",
          "fields": [
            "id^2",
            "description"
          ]
        }
      },
      "max_boost": 10, 
      "functions": [
        {
          "field_value_factor": {
            "field": "downloadCount",
            "factor": 0.00001
          }
        }
      ]
    }
  }
}
```

---

# Part 20 of 20

<!-- $size: A2 -->

We need to add fuzziness parameter!
```
GET nuget/_search
{
  "query": {
    "function_score": {
      "query": {
        "multi_match": {
          "query": "elastiscearch",
          "fuzziness": 1,
          "fields": [
            "id^2",
            "description"
          ]
        }
      },
      "max_boost": 10, 
      "functions": [
        {
          "field_value_factor": {
            "field": "downloadCount",
            "factor": 0.00001
          }
        }
      ]
    }
  }
}
```

---
<!-- $size: 16:9 -->

# Inverted index 1/2

What it looks like:
https://gist.github.com/robertlyson/98f015868c9652db03300f794482bdab#what-is-relevant-search

---

# Inverted index 2/2

How it works in action:
https://sketchboard.me/BA33PjFmAmyz#/

---

## [Exercise 1 - relevant search](#exercise1)

---

```git clone https://github.com/robertlyson/RelevantSearch.git```

and after build run **RelevantSearch.DataIndexer** app

---

# part 1 of 7 - match

```git checkout part1```

Job to do:
We are looking for branches with **LocationName** containing **morgan**.

---

# part 2 of 7 - terms

```git checkout part2```

Job to do:
Find branch with zip code **48827-3158**.

---

# part 3 of 7 - multi match

```git checkout part3```

Job to do:
Find branches which **LocationName** contain at least **jp** and **morgan**.

---

# part 4 of 7 - I'm not afraid of typos!

```git checkout part4```


Job to do:
Make our search typo aware. For query **mogran** we want to match branches with **LocationName** containing **morgan**.

---

# part 5 of 7 - boosting some fields

```git checkout part5```

Job to do:
**LocationName** should be more importan than **LocationContact**. 

---

# part 6 of 7 - looking for synonyms

```git checkout part6```

Job to do:
Find branch **Kozey and Sons** by it's synonym **K and S**.

---

# part 7 of 7 - joining it all together


```git checkout part7```

Job to do:
Combine together search technics: boosting, typos awareness and synonyms. 

---

## Geo Search

---

## Geo Point - hands on .. kibana

```json
DELETE attractions

PUT attractions

PUT attractions/_mapping/restaurant
{
  "properties": {
    "name": {
      "type": "text"
    },
    "location": {
      "type": "geo_point"
    }
  }
}
```

---

## Index some data 1/3

There are three formats for ```geo_point```:

```json
PUT /attractions/restaurant/1
{
  "name":     "Chipotle Mexican Grill",
  "location": "40.715, -74.011" 
}
```

```40.715, -74.011``` -> lat,lon

---

## Index some data 2/3

```json
PUT /attractions/restaurant/2
{
  "name":     "Pala Pizza",
  "location": { 
    "lat":     40.722,
    "lon":    -73.989
  }
}
```

---

## Index some data 3/3

```json
PUT /attractions/restaurant/3
{
  "name":     "Mini Munchies Pizza",
  "location": [ -73.983, 40.719 ] 
}
```

```[-73.983, 40.719]``` -> lon,lat!

---

## Filtering by geo point

- **geo_bounding_box**
Find geo-points that fall within the specified rectangle.
- **geo_distance**
Find geo-points within the specified distance of a central point.
- geo_distance_range
Find geo-points within a specified minimum and maximum distance from a central point.
- geo_polygon
Find geo-points that fall within the specified polygon. This filter is very expensive. If you find yourself wanting to use it, you should be looking at geo-shapes instead.

---

## Geo Bounding Box

```json
GET /attractions/restaurant/_search
{
  "query": {
    "bool": {
      "filter": {
        "geo_bounding_box": {
          "location": { 
            "top_left": {
              "lat":  40.8,
              "lon": -74.0
            },
            "bottom_right": {
              "lat":  40.7,
              "lon": -73.0
            }
          }
        }
      }
    }
  }
}
```

---

## Geo Distance Filter

```json
GET /attractions/restaurant/_search
{
  "query": {
    "bool": {
      "filter": {
        "geo_distance": {
          "distance": "1km", 
          "location": { 
            "lat":  40.715,
            "lon": -73.988
          }
        }
      }
    }
  }
}
```

---

## Sorting by distance

```json
GET /attractions/restaurant/_search
{
  "query": {same as prev},
  "sort": [
    {
      "_geo_distance": {
        "location": {
          "lat": 40.715,
          "lon": -73.988
        },
        "unit": "km",
        "order": "asc"
      }
    }
  ]
}
```

---

## Geo Aggregations 1/2

```json
GET /attractions/restaurant/_search
{
  "size": 0,
  "aggs": {
    "geo": {
      "geo_distance": {
        "field": "location",
        "unit": "km", 
        "origin": {
          "lat": 40.715,
          "lon": -73.988
        },
        "ranges": [
          {
            "from": 0,
            "to": 1
          }
        ]
      }
    }
  }
}
```

---

## Geo Aggregations 2/2

```json
GET /attractions/restaurant/_search
{
  "size": 0,
  "aggs": {
    "geo": {
      "geohash_grid": {
        "field": "location",
        "precision": 10
      }
    }
  }
}
```

Geohash? What the hell?(Next page) 

---

<img src="https://dpzbhybb2pdcj.cloudfront.net/turnbull/Figures/04fig06_alt.jpg" alt="drawing" width="40%"/>
(Source: relevant search book)

More detailed description: http://www.bigfastblog.com/geohash-intro

---

## [Exercise 2 - coffeshop finder](#exercise2)

```git clone https://github.com/robertlyson/CoffeeFinderApp.git```

---

# part 1 of 4

```git checkout exercise1```

Fix ```DataHasBeenIndexedIntoElasticsearch``` test.

Job to do:
- indexing documents
- refresh index
- bulk index to speed up

---

# part 2 of 4

```git checkout exercise2```

Fix ```LocationIsOfTypeGeoPoint``` test.

Job to do:
- mapping - ```location``` field should be of type ```goe_point```

---

# part 3 of 4

```git checkout exercise3```

Fix ```FindCoffeeNearMe``` test.

Job to do:
- write query to find coffee shops near our location - ```GeoDistance``` filter
- add sort to find nearest places

---

# part 4 of 4

```git checkout exercise4```

Fix ```FindMyFavCoffeeNearMe``` test.

Job to do:
- find by name - ```BoolQuery``` with ```match``` query and ```GeoDistance``` filter

---

## Percolate Query 1 of 4

What is that?

--- 

## Percolate Query 2 of 4

Standard flow: 
**Document** -> indexing -> query -> searching -> do we have matching **documents** for **query**?

---

## Percolate Query 3 of 4

Percolate flow: 
**Query** -> indexing -> document -> searching -> do we have matching **queries** for **document**?

---

## Percolate Query 4 of 4

For what I can use it?

---

## Percolate Query - hands on 1 of 4

```json
PUT /my-index
{
    "mappings": {
        "_doc": {
            "properties": {
                "message": {
                    "type": "text"
                },
                "query": {
                    "type": "percolator"
                }
            }
        }
    }
}
```

---

## Percolate Query - hands on 2 of 4

```json
PUT /my-index/_doc/1?refresh
{
    "query" : {
        "match" : {
            "message" : "bonsai tree"
        }
    }
}
```

---

## Percolate Query - hands on 3 of 4

```json
GET /my-index/_search
{
    "query" : {
        "percolate" : {
            "field" : "query",
            "document" : {
                "message" : "A new bonsai tree in the office"
            }
        }
    }
}
```

---

## Percolate Query - hands on 4 of 4

```json
GET /my-index/_search
{
    "query" : {
        "percolate" : {
            "field" : "query",
            "document" : {
                "message" : "robert"
            }
        }
    }
}
```

---

## [Exercise 3 - percolate query](#exercise3)

Monitoring prices of store items.
Flow diagram: https://sketchboard.me/HA4nBAR8smyp#/

---

# part 1 of 3

Job to do:
Create index with proper mapping.

```MappingIsCreatedCorrectly``` should by green.

---

# part 2 of 3

Job to do:
Register query as percolate query.

Fix ```RegisterPriceAlertQuery``` test.

---

# part 3 of 3

Job to do:
Check if there are any percolate documents registered in elasticsearch.

Make sure ```UpdatingTeslaItemTo90ShouldRiseAlert``` test is passing.

---

## [I want more](#i-want-more)

- Elasticsearch: The Definitive Guide https://www.elastic.co/guide/en/elasticsearch/guide/2.x/index.html
- Relevant Search: https://www.manning.com/books/relevant-search
- NuSearch: https://github.com/elastic/elasticsearch-net-example

---

## Last word

Please give us your feedback! https://bit.ly/2sX7MjB

![image](https://cdn.discordapp.com/attachments/434287216068395008/449467777229783051/static_qr_code_without_logo.jpg)

---
