# Distributed Multilevel Caching in Micro-services

Primer : Distributed Caching & Simply what we call  Caching are two different things. Distributed caching stands to the design of caching  where that entire data is cached in parts or as whole in different locations to have more locality of data, there by increasing performance. So here we start with question.

## Why distributed caching? 

In this era of micro services & server-less computing, we create numerous services which needs to communicate & collaborate together to serve requests. This causes lots of I/O operations & calls. Usually we use caching to to reduce I/O. Generally we know about caching. Here we discuss how to effectively use caching so that we can get meet scale & efficiency. Idea is taken from CPU caches.
Below is diagram that as a primer for caching in CPU

![Cpu - caching](./Doc/images/cpucaching.jpg)


## How this applies to Micro-services ? 

Lets consider an scenario of a health care portal, where we have to show certain information from different sub systems/services

1. Net total of items/services bought - fetched using transaction/order ID
2. Subscription & details owned by the user - fetched by subscription ID
3. User features , permission, etc, etc - from user ID
4. Associated User Account of family member & plans - Using Group ID
   
Here you can see that some information (ex: '2,3' item in above list) is required by many services & is used under very high frequency where as some items are of lower precedence, but again is candidate for caching. So one type of caching won't fit for all, and will eventually cause unwanted issues especially under load conditions ,also  as we will be hitting too much IO or systems limits. say if we are using Redis , the latency & availability incorporated is high when compared to in Memory Collection

## What should be the right approach ?

I should remind the fact that  caching should be the last resort in achieving performance, our strategy here is to hit a sweet spot by using multi level caching & fallback to actual store. Very high frequently accessed data are used can be stored "in Memory Cache - L1 level distributed cache " then either paged or reflected to "L2 Cache - ex :Redis, Apache ignite,ncache etc". Here the advantage is that there can be multiple algorithms & strategies in each level to enhance efficiency. One crucial advantage is that other services can observe event streams and do additional data  prefetching  as required. This approach lets us operate our caching across services as efficient as possible. One prominent criteria is that L1 cache has to be of Distributed in manner, so that at least invalidation of data should be done, otherwise it will cause consistency issues.

![sample flow diagram ](./Doc/images/image1.png)
![sample flow diagram ](./Doc/images/flowdiagram.png)

## Is there a Sample implementation

       Yes, Refer a sample implementation in C# .NetCore using L1 in memory & L2 Redis.

## Getting Started
 
