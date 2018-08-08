### Introduction ###

The goal of this RESTful API is to allow our development team to have a way to develop against DataLake projects. Our previous experiences with Hadoop projects showed us how important having a good test environment can tremendously increase our productivity and allow us to deliver value in the shortest time possible. 

### Setup ###

- build with .Net Core as a technology
- hosted in a remote box (PrismDev210) box.
- SQL Database is Hosted in your SQL Database server machine box 
- Ports:
  * 1000 for http request
  * 1234 for https request (though I doubt this will work for now)
  
### Entry Points ###

There are two main entry points that are available to all users
* TagDefs: this controller is responsible for everything that is related to retrieving information about a Tag
* TagHists: this controller is all about historical value retrieval
Most of the calls supports pagination. so when issuing a Get request the user can add the "pagination" in the header with the value (page_number, record_per_page)

### Tag Retrieval ###

In order to retrieve tags definition and current value we should:
* set the entry point to: http://host_machine:1000/api/TagDefs
* issue the Get Request
* Response is a json representation of the Tags

Description	| Pagination Support | EndPoint |	Response
------------|-------------|-------------|-------------
GET All Tags available |	Yes | 	http://prismdev210:1000/api/TagDefs | ```[{"id": 1,"source": "eDNA","name": "PRISMLAKE_01","description": "Prism Data Lake Point 01","extendedDescription": "Prism Data Lake Point 01 Extended Description","value": 58.099195760720967,"status": 1,"timeStamp": "2018-05-16T09:45:13"}]```
GET a tag by its ID	| No	| http://prismdev210:1000/api/TagDefs/{id} e.g: http://prismdev210:1000/api/TagDefs/1 | {"id": 1,"source": "eDNA","name": "PRISMLAKE_01","description": "Prism Data Lake Point 01","extendedDescription": "Prism Data Lake Point 01 Extended Description","value": 76.221202395959381,"status": 1,"timeStamp": "2018-05-16T09:50:34"}
Get a Tag by tagname	| No | http://prismdev210:1000/api/TagDefs/TaxonomybyTagName/{TagName} e.g: http://prismdev210:1000/api/TagDefs/TaxonomybyTagName/PRISMLAKE_01 | {"id": 1,"source": "eDNA","name": "PRISMLAKE_01","description": "Prism Data Lake Point 01","extendedDescription": "Prism Data Lake Point 01 Extended Description","value": 79.5088214238681,"status": 1,"timeStamp": "2018-05-16T09:53:15"}
Get All tags from the same source	| No | http://prismdev210:1000/api/TagDefs/TagsNameList/{PlantName} e.g: http://prismdev210:1000/api/TagDefs/TagsNameList/pi | [{ "id": 2,"source": "pi", "name": "PRISMLAKE_02","description": "Prism Data Lake Point 02","extendedDescription": "Prism Data Lake Point 02 Extended Description","value": 35.420317219300344, "status": 1, "timeStamp": "2018-05-16T09:55:36"}]
Get Current Value for a Tag	| No	| http://prismdev210:1000/api/TagDefs/TagsCurrentValue/{TagName} e.g: http://prismdev210:1000/api/TagDefs/TagsCurrentValue/PRISMLAKE_01 | { "id": 1,"source": "eDNA", "name": "PRISMLAKE_01", "description": "Prism Data Lake Point 01", "extendedDescription": "Prism Data Lake Point 01 Extended Description", "value": 42.098769052931466,"status": 1,"timeStamp": "2018-05-16T09:58:16"}

### Historical Data Retrieval ###

In order to retrieve Historical values we should:
* set the entry point to: http://prismdev210:1000/api/TagHists
* issue the Get Request
* Response is a json representation of the Historical records for that Tag





### Troubleshoot ###

**Resetting the Database:**
The app is located under the folder C:\inetpub on the Prismdev210 machine in order to be accessible to anyone that is currently logged in
* Navigate to the folder C:\inetpub\bigdataboost\BigDataBoost.API
* Make sure that the Database is already created
* add a migration to the DB by running the command dotnet ef migrations add "initial"
* Update the database with the command dotnet ef database update

Launching the App in the self hosted environment
* Navigate to the folder C:\inetpub\bigdataboost\BigDataBoost.API
* >**dotnet run**
```
Using launch settings from C:\inetpub\bigdataboost\BigDataBoost.API\Properties\l
aunchSettings.json...
Hosting environment: Development
Content root path: C:\inetpub\bigdataboost\BigDataBoost.API
Now listening on: http://*:1000
Now listening on: https://*:1234
Application started. Press Ctrl+C to shut down.
```

Running the app
   
 
