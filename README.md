# Major Project "Remote Healthcare"
Created by me, and 3 other students

## Project Description
- This year, we started off learning app development in C#. We had to build 3 applications: client, docter and a server.
- The docter is able to start a remote health training session in the app. The client has vr-goggles and is sat on an exercise bike. The client can see his speed, heartrate, time and chat messages in vr on a virtual phone mounted to the bike. The client can cycle through a custom made farm land. The faster he cycles, the faster he goes. All the information is being logged and sent to the server, which stores the data safely and also sends it to the docter application. In the docter application, the docter sees graphs with the data, and is able to send messages, or stop the session.
- The vr was ran on a vr server at school. We were also able to have a vr simulator on our pc and connect with it, so that we would not have to wear goggles every time.
- All the communication went via the server, which was also encrypted using TLS. Docter cridentials and patient data was also encrypted, however we had issues with the logging functionality on the server, so we removed it.
- This project also included creating our own protocol for communication between the applications. We also had to figure out how to communicate with the vr server.
- We also had to figure out how to communicate with the exersice bike, and the heart rate monitor (Garmin) using Bluetooth. 

## Grade
7.2 / 10

## Date
Year 2 Period 1 | Nov 1, 2023 11:59pm
