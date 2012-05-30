PhillySNUG-Clients
==================

Code for the client-side of the demo

There are two projects in this solution. The RealWorld.ApiClient is a client at "Level 2" of the Richardson REST Maturity model. It is what is "generally" accepted as a RESTful application. Since the version of the API that it communicates with is not strictly speaking, a RESTful API, then it is also not a true RESTful application.

The RealWorld.RestClient project contains an application that communicates with a fully implemented REST archtectural style. Both the client and the API incorporate "hypermedia" links to allow for loosely-coupled development of the client & server side applications. It is missing the use of a custom media type for the structure of the API messages for simplicity of the demonstration. Implementing a custom media type is left as an exercise for the reader.