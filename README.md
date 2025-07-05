\# 🩸 BloodConnect



\*\*BloodConnect\*\* is a campus-based blood donation management web application built with ASP.NET Core MVC. It enables students to connect with blood donors in under a minute, making the life-saving process faster and more efficient.



🌐 \*\*Visit\*\*: \[https://bconnect.live](https://bconnect.live)



---



\## 📌 Features



\- 🔐 Secure user registration with email verification

\- ✅ Admin approval system for new users

\- 🩸 Request blood with reason, location, and phone number

\- 📨 Email notifications (request sent, accepted, declined, completed)

\- ⭐ Points system and donor leaderboard

\- 📊 Admin dashboard with real-time statistics

\- 📷 Profile picture support via Cloudinary

\- 🔍 Donor search, filter, and pagination

\- 🛡️ Role-based authorization (Admin/User)

\- 🚫 Donation restriction: Can't request same donor within 4 months



---



\## 🛠️ Tech Stack



| Technology         | Purpose                            |

|--------------------|-------------------------------------|

| ASP.NET Core MVC   | Backend Framework                   |

| Entity Framework   | ORM and Database Access             |

| Microsoft Identity | Authentication and Role Management |

| SQL Server         | Database                            |

| Bootstrap 5        | Frontend Styling                    |

| Cloudinary         | Profile Image Uploads               |

| Gmail SMTP         | Email Notifications                 |



---



\## 🚀 Getting Started



\### 1. Clone the Repository



```bash

git clone https://github.com/dabananda/BloodConnect.git

cd BloodConnect

```



\### 2. Configure `appsettings.Development.json`



```json

{

&nbsp; "ConnectionStrings": {

&nbsp;   "DefaultConnection": "Your SQL Server connection string"

&nbsp; },

&nbsp; "Cloudinary": {

&nbsp;   "CloudName": "your-cloud-name",

&nbsp;   "ApiKey": "your-api-key",

&nbsp;   "ApiSecret": "your-api-secret"

&nbsp; },

&nbsp; "EmailSettings": {

&nbsp;   "SmtpServer": "smtp.gmail.com",

&nbsp;   "Port": 587,

&nbsp;   "SenderName": "BloodConnect",

&nbsp;   "SenderEmail": "your-email@gmail.com",

&nbsp;   "Password": "your-app-password"

&nbsp; }

}

```



\### 3. Run the App



```bash

dotnet ef database update

dotnet run

```



Visit: `https://localhost:5001`



---



\## 👨‍💻 Contributing



We welcome contributions from developers of all levels!



\### How to contribute:



1\. Fork this repository

2\. Create a new branch (`feature/YourFeature`)

3\. Commit your changes

4\. Submit a Pull Request



Check out the \*\*\[Issues](https://github.com/dabananda/BloodConnect/issues)\*\* tab for suggestions.



---



\## 📄 License



This project is open-source and available under the \[MIT License](LICENSE).



---



\## 🙏 Acknowledgments



Created with ❤️ by \[Dabananda Mitra](mailto:imdmitra@gmail.com)  

🎓 8th Semester, CSE (2019-20), Institute of Science Trade \& Technology (ISTT), Mirpur 15, Dhaka



