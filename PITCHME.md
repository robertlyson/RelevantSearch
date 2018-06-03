# Elasticsearch & .Net

![](https://www.elastic.co/static/images/elastic-logo-200.png)

---
<!-- page_number: true -->

# Outline

- [Preparing Environment](#preparing-environment)
- [But, why elasticsearch?](#but-why-elasticsearch) 
- [Let's meet elasticsearch](#lets-meet-elasticsearch)
- [What is relevant search?](#what-is-relevant-search)
- [Exercise 1 - relevant search](#exercise1)
- [Exercise 2 - coffeshop finder](#exercise2)
- [Exercise 3 - percolated query](#exercise3)
- [I want more](#i-want-more)

---

# [Preparing Environment](#preparing-environment)

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

# 1/7

Requirement was simple, allow users to search for branches* by name. 
So we did it!

*this is how banks like to call theirs locations(ATM, department, office)

---

# 2/7

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

# 3/7

Worked very well.

![image](https://user-images.githubusercontent.com/2392583/40502596-bb3dd750-5f8b-11e8-96e7-eaea5ac99c0a.png)

---

# 4/7

Unfortunately, few days later after we deployed this feature to users, it turned out something is wrong. Someone typed `Morgan` in search box and got as a result:

![image](https://user-images.githubusercontent.com/2392583/40502676-fe739c4e-5f8b-11e8-8bb9-ece82b83f7e8.png)

---

# 5/7

WAT. There is no even `m` in this location name!

After some time spent on investigating the issue, we discovered someone extended above query with one more case

---

# 6/7

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

# 7/7

```sql
or LOWER(LocationContact) like '%morgan%'
```

![image](https://media.giphy.com/media/3osxYoT363g5uv72bm/giphy.gif)'

We will try to fix it later on .. I promise.

---

# use cases 1/2

- realtime search
- very flexible/configurable search
- faceted search (https://www.euro.com.pl/zlewozmywaki,_Schock,g!zlewozmywak-okragly.bhtml)
- reporting
- alerting
- monitoring(logstash, beats, kibana)
- matching systems
- autocompletion

---

# use cases 2/2

today:
- very flexible/configurable search
- alerting

---

## [Let's meet elasticsearch](#lets-meet-elasticsearch)

---

some facts about elasticsearch:
- based on lucene
- simplify very complex lucene API with REST API
- distributed by nature

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

# 1/6

curl from cmd 
```
curl -XGET http://localhost:9200
```

---

# 2/6

kibana and dev tools

![image](https://user-images.githubusercontent.com/2392583/40768166-cc7e8cca-64b4-11e8-9c48-1751dc5d49a8.png)
This is a simple query returning 10 documents from all ES indices.

---

# 3/6

elasticseatch clients - in our case that will be NEST

---

# 4/6

Client init:
```csharp
var uri = new Uri("http://localhost:9200");
var settings = new ConnectionSettings(uri);
settings.DefaultIndex(IndexName); // so we don't need to specify index with each call on elasticClient
settings.EnableDebugMode(); // because we want to see request/response body in IResponse from elasticClient
var client = new ElasticClient(settings);
```

---

# 5/6

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

# 6/6

Obiect initializer syntax:
```
var searchRequest = new SearchRequest<Project>
{
    Query = new MatchAllQuery()
};
```

---

## Indexing documents

---

# 1/3

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

# 2/3

In NEST you can index document by doing(I'll do examples with fluent api)

```csharp
elasticClient.Index(
    new Employee
    {
        FirstName = "John",
        LastName = "Smith",
        Age = 25,
        About = "I love to go rock climbing",
        Interests = new[] {"sports", "music"}
    }, d => d.Index("megacorp").Type("employee"));
```

---

# 3/3

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

## [What is relevant search?](#what-is-relevant-search)

---

Relevance search is ...

- what is relevant search?
- inverted index is the key
- index mapping
- search: full text, exact


---

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
We are looking for branches with **LocationName** containing phrase **morgan**.

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
**LocationName** should be more importan than **LocationContant**. 

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

- geo_bounding_box
Find geo-points that fall within the specified rectangle.
- **geo_distance**
Find geo-points within the specified distance of a central point.
- geo_distance_range
Find geo-points within a specified minimum and maximum distance from a central point.
- geo_polygon
Find geo-points that fall within the specified polygon. This filter is very expensive. If you find yourself wanting to use it, you should be looking at geo-shapes instead.

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

Geohash? http://www.bigfastblog.com/geohash-intro

---

## [Exercise 2 - coffeshop finder](#exercise2)

---

# part 1 of 1

Fix ```DataHasBeenIndexedIntoElasticsearch``` test.

Job to do:
- indexing documents
- refresh index
- bulk index to speed up

---

# part 2 of 2

Fix ```LocationIsOfTypeGeoPoint``` test.

Job to do:
- mapping - ```location``` field should of type ```goe_point```

---

# part 3 of 3

Fix ```FindCoffeeNearMe``` test.

Job to do:
- write query to find coffe shops near codeschool - ```GeoDistance``` filter
- add sort to find nearest places

---

# part 4 of 4

Fix ```FindMyFavCoffeeNearMe``` test.

Job to do:
- find by name - ```BoolQuery``` with ```match``` query and ```GeoDistance``` filter

---

## [Exercise 3 - percolated query](#exercise3)

---

## [I want more](#i-want-more)

- Elasticsearch: The Definitive Guide https://www.elastic.co/guide/en/elasticsearch/guide/2.x/index.html
- NuSearch https://github.com/elastic/elasticsearch-net-example

---

## Last word

Please give us your feedback! [Click](https://docs.google.com/forms/d/16R62Q5J6zpd5LtMCGs1LbNVMQ8uffW_JQ6yKzMKae4s/viewform?edit_requested=true)

![image](https://cdn.discordapp.com/attachments/434287216068395008/449467777229783051/static_qr_code_without_logo.jpg)

---
