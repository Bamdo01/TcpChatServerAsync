# TcpChatServerAsync: 비동기 TCP 채팅 서버 V2

## 🚀 프로젝트 소개

이 프로젝트는 기존의 **[동기식, 스레드 기반 채팅 서버(V1)](https://github.com/Bamdo01/TcpChatServerSync)**를 **NetMQ** 라이브러리를 활용하여 **완전 비동기 방식**으로 재구성한 버전입니다.

전통적인 '클라이언트당 스레드 할당' 방식의 한계를 극복하고, 적은 리소스로 더 많은 동시 연결을 처리할 수 있는 확장성 높은 서버 아키텍처를 학습하는 것을 목표로 합니다.

<br>

## ✨ V1(동기식) 대비 주요 개선점

| 구분 | V1 (동기식 서버) | V2 (비동기 서버) - **개선된 점** |
| :--- | :--- | :--- |
| **코어 아키텍처** | `TcpListener` + 클라이언트당 `Thread` 생성 | **NetMQ 비동기 메시징 패턴** |
| **요청 처리** | 각 스레드가 동기 I/O로 블로킹 | **`Router` 소켓**으로 비동기 요청 수신 |
| **채팅 처리** | 모든 메시지를 동일한 스트림으로 처리 | **`Pull` 소켓**으로 채팅 메시지 전문 수신 |
| **브로드캐스트** | `lock`을 이용해 전체 클라이언트 목록 순회 | **`Publisher` 소켓**으로 효율적인 브로드캐스팅 |
| **자원 관리** | 다수의 스레드 생성 및 컨텍스트 스위칭 오버헤드 | **`NetMQPoller`**가 단일 스레드로 모든 소켓 관리 |
| **확장성** | 동시 접속자 수에 따라 스레드 수가 늘어나 성능 저하 | **뛰어난 확장성**, 적은 리소스로 대규모 연결 처리 |

<br>

## 📑 주요 기능

V1의 핵심 기능은 그대로 유지하며, 내부 구현을 비동기 방식으로 개선했습니다.

* **사용자 인증**: Salt와 SHA256 해시를 이용한 안전한 회원가입 및 로그인
* **실시간 채팅**: NetMQ의 Pub-Sub 패턴을 이용한 실시간 메시지 브로드캐스팅
* **데이터베이스 연동**: MySQL에 사용자 정보 및 모든 채팅 로그 저장 (SQL Injection 방지를 위한 파라미터화 쿼리 사용)
* **서버 관리**: 서버 콘솔에서 `online` (접속자 확인), `exit` (서버 종료) 명령어 지원

<br>

## 🛠️ 기술 스택

* **언어 및 프레임워크**: C# (.NET)
* **비동기 메시징**: **NetMQ**
* **데이터베이스**: MySQL (with `MySqlConnector`)

<br>

## 💾 데이터베이스 스키마

서버 시작 시 아래 테이블들이 자동으로 생성됩니다.

* **사용자 정보 테이블 (`users`)**
    ```sql
    CREATE TABLE IF NOT EXISTS users (
        id INT PRIMARY KEY AUTO_INCREMENT,
        userid VARCHAR(50) NOT NULL UNIQUE,
        password VARCHAR(100) NOT NULL,
        salt VARCHAR(100) NOT NULL,
        phone VARCHAR(20),
        gender VARCHAR(10) NOT NULL,
        birthdate DATE NOT NULL
    );
    ```
* **채팅 메시지 로그 테이블 (`chat_messages`)**
    ```sql
    CREATE TABLE IF NOT EXISTS chat_messages (
        id INT PRIMARY KEY AUTO_INCREMENT,
        sender_id VARCHAR(50) NOT NULL,
        message TEXT NOT NULL,
        sent_at DATETIME DEFAULT CURRENT_TIMESTAMP
    );
    ```

<br>

## 🚀 실행 방법

1.  **Prerequisites**: .NET SDK, MySQL Server가 설치되어 있어야 합니다.
2.  MySQL에 `NEXTCHAT` 데이터베이스를 생성합니다.
3.  `Database/DbManager.cs` 파일의 연결 문자열(`connStr`)을 자신의 DB 환경에 맞게 수정합니다.
4.  프로젝트를 빌드하고 실행합니다.
5.  서버 콘솔에 아래와 같이 포트 정보가 표시되면 정상적으로 실행된 것입니다.

    ```
    [서버] ROUTER:5555 / PULL:5556 / PUB:5557 열림
    ```

    * **Router (요청 처리용)**: `5555`
    * **Pull (채팅 수신용)**: `5556`
    * **Publisher (채팅 발송용)**: `5557`

