# Elasticsearch & .Net

![](https://www.elastic.co/static/images/elastic-logo-200.png)

---

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

Unfortunately, few days later after we deployed this feature to users, it turned out something is wrong. Someone typed `Morgan` in search box and got as a result.

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

![image](https://media.giphy.com/media/3osxYoT363g5uv72bm/giphy.gif)

---

# use cases 1/2

- very flexible/configurable search
- faceted search (https://www.euro.com.pl/zlewozmywaki,_Schock,g!zlewozmywak-okragly.bhtml)
- reporting
- alerting
- monitoring
- matching systems
- autocompletion

---

# use cases 2/2

today:
- very flexible/configurable search
- alerting
- some facts about elasticsearch
- based on lucene(extends it with easier api, distributed nature)

---

## [Let's meet elasticsearch](#lets-meet-elasticsearch)

---

some facts about elasticsearch:
- based on lucene
- simplify very comples lucene API with REST API
- distributed by nature

---