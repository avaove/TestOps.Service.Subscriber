As part of my internship at [Ceridian](https://www.ceridian.com/) in the Test Platform Services team, I built this service and hosted it in our AKS cluster. This service is part of our transition from a monolithic system to a microservices architecture.

# Purpose
The service establishes subscriptions to various topics or channels within message queuing frameworks we use (Redis, Kafka and potentially other frameworks) using a pub/sub pattern. These subscriptions are made with call-back functions that are designated to trigger upon message
reception. This service will be the core asynchronous event handling and communication between different components of our system.

When a message is published to a subscribed topic or channel, the Subscriber intercepts the message and subsequently triggers the
associated call-back function. This creates real-time response to messages and the execution of necessary actions based on the content of
those messages.

The code makes use of libraries private to Ceridian and therefore cannot be built and ran, but if you want to learn more about how this service is used in our system, in the [refer to this flowchart here](https://miro.com/app/board/uXjVMxHwDYw=/)!
