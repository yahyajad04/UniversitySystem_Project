University Management System (Web API & MVC)
This project is a comprehensive University Management System built during my Full Stack Development course at Coderz for Software and Training. It is designed to mirror the complex enrollment and financial logic of a real-world university, focusing on Role-Based Access Control (RBAC) and secure data management.



The Challenge
The goal was to move beyond a simple "To-Do list" and build a system where data is deeply interconnected. I wanted to handle the "trickle-down" effects of university actions—like how a teacher finishing a course automatically triggers grade calculations and financial updates for dozens of students simultaneously.




Tech Stack

Backend: ASP.NET Core Web API 




Frontend: ASP.NET MVC (Client-side) 



Security: JWT (JSON Web Tokens) & ASP.NET Identity 



Styling: Bootswatch & AdminLTE Dashboard 



Tools: Entity Framework Core, SQL Server, HttpClient 


User Roles & Logic
The system is strictly controlled; anonymous registration is disabled to maintain academic integrity.


1. The Admin (The Architect)

Full CRUD: Manages Teachers, Students, and the University's available Majors.


Gatekeeping: Approves courses created by teachers before they go live.

Financials: Manages student receipts and teacher salaries.

2. The Teacher (The Instructor)
Course Creation: Teachers propose courses and define which specific Majors are eligible to enroll.


Grade Management: Full control over First, Second, and Final exam grades.

The "Finalize" Workflow: I implemented an "End Course" feature. Once triggered:

Grades become "Official" and locked from further editing.

Letter grades are automatically calculated for students.

Financial balances and credit hours are updated across the system.

3. The Student (The Learner)
Dynamic Enrollment: Students only see courses that match their specific Major.

Real-time Billing: Enrolling in a course automatically updates the student’s "Receipt" based on the Major's hourly cost.

Academic History: A dedicated "Finished Courses" view tracks total hours completed successfully.

What I Learned
Complex State Management: Managing the transition of a course from "Proposed" to "Approved" to "Finished."

Calculated Data: Learning that some data (like a student's total hours) should be dynamically updated based on specific triggers rather than just static input.


Security First: Implementing JWT ensured that a student couldn't accidentally (or intentionally) access the Admin's financial endpoints.


How to Run
Database: Update the connection string in appsettings.json and run Update-Database via Package Manager Console.

Seeding: The system automatically seeds the initial Admin account on the first run.

API: Start the Web API project first.

Client: Start the MVC project to begin the simulation.
