# 🏠 Room Digital Twin

> A real-time IoT system that mirrors a physical room into a fully walkable VR environment on **Meta Quest 3** — powered by ESP32 sensors, Firebase Realtime Database, and Unity URP.

<p align="center">
  <img src="https://img.shields.io/badge/Unity-6000.3.6f1-black?logo=unity&logoColor=white"/>
  <img src="https://img.shields.io/badge/ESP32-Arduino-red?logo=arduino&logoColor=white"/>
  <img src="https://img.shields.io/badge/Firebase-Realtime DB-orange?logo=firebase&logoColor=white"/>
  <img src="https://img.shields.io/badge/Meta-Quest 3-blue?logo=meta&logoColor=white"/>
  <img src="https://img.shields.io/badge/Platform-Android / XR-green"/>
  <img src="https://img.shields.io/badge/License-MIT-purple"/>
</p>

---

> [!NOTE]
> **This is a concept-level Digital Twin project for home/hobbyist use — not an enterprise-grade simulation.**
> The main idea is to demonstrate the **bridge between physical hardware and cloud**, and show how a **Unity VR application can run purely off cloud data — completely independent of the hardware**.
> Once the ESP32 pushes sensor data to Firebase, the Unity app doesn't care where the data came from — it could be real sensors, a phone, a script, or anything that writes to the same Firebase path. The VR environment reacts to **cloud state**, not hardware directly. This decoupling is the core concept.

---

## 📌 What Is This?

This project creates a **live digital twin of a physical room**. Sensors on an ESP32 microcontroller read the room's temperature, humidity, light level, and motion — then stream that data to Firebase every **2 seconds**. A Unity VR app on Meta Quest 3 reads this Firebase data and **reacts in real time**:

- 🌡️ **Walls change color** based on temperature (cool blue → hot red)
- 💡 **Lights dim and brighten** based on LDR light readings
- 🟡 **Floor glows yellow** when motion is detected
- 📺 **UI panels show live sensor values** in the VR room

---

## 🏗️ System Architecture

```
[DHT11] ──┐
[PIR]   ──┼──► [ESP32 Dev Board] ──WiFi/HTTP──► [Firebase RTDB] ──REST──► [Unity C#] ──► [Meta Quest 3 APK]
[LDR]   ──┘         (Arduino)                   (Cloud DB)                  (URP + XR)       (VR Headset)
```

See `architecture.svg` for the full visual diagram.

---

## 🛠️ Hardware

| Component | Model | Pin | Cost |
|-----------|-------|-----|------|
| Microcontroller | ESP32 Robocraze Dev Board (ESP32-D0WD-V3) | — | ₹529 |
| Temp + Humidity | DHT11 | GPIO4 (D4) | ₹159 |
| Motion Sensor | PIR HC-SR501 × 2 | GPIO27 (D27) | ₹278 |
| Light Sensor | LDR Photoresistor + 10kΩ resistor | GPIO34 (D34) | ₹189 |
| Prototyping | Breadboard 400 tie + jumper wires | — | ₹844 |
| Cable | Micro USB | — | ₹80 |
| **Total** | | | **₹2,089** |

---

## 🔌 Wiring Guide

> Color convention: 🔴 Red = Power | ⚫ Black = GND | 🟡 Yellow = Signal

### DHT11
| DHT11 Pin | Wire | ESP32 Pin |
|-----------|------|-----------|
| VCC | Red | 3V3 |
| DATA | Yellow | D4 (GPIO4) |
| GND | Black | GND |

### PIR HC-SR501
> Dome facing you: Left = VCC, Middle = OUT, Right = GND

| PIR Pin | Wire | ESP32 Pin |
|---------|------|-----------|
| VCC | Red | VIN (5V) |
| OUT | Yellow | D27 (GPIO27) |
| GND | Black | GND |

> ⚠️ Set PIR Time Delay pot fully **anti-clockwise** for 3–5s delay. Jumper in **H mode**.

### LDR Voltage Divider
| Component | Connection |
|-----------|------------|
| LDR Leg 1 | 3V3 |
| LDR Leg 2 + 10kΩ Leg 1 | Same row → D34 (GPIO34) |
| 10kΩ Leg 2 | GND |

### Auto-Reset Capacitor
| | |
|---|---|
| 10µF capacitor **long leg (+)** | EN pin |
| 10µF capacitor **short leg (−)** | GND pin 2 |

---

## ☁️ Firebase Setup

1. Go to [Firebase Console](https://console.firebase.google.com/) → Create project
2. Enable **Realtime Database** → Start in **test mode**
3. Note your Database URL: `https://YOUR-PROJECT-default-rtdb.REGION.firebasedatabase.app`
4. Set rules:
```json
{
  "rules": {
    ".read": true,
    ".write": true
  }
}
```

Expected data structure in Firebase:
```
room/
  ├── temperature: 29.0
  ├── humidity: 41.0
  ├── light: 1420
  └── motion: 1
```

---

## 📟 ESP32 Code Setup

### Libraries Required (Arduino IDE)
| Library | Author | Version |
|---------|--------|---------|
| DHT sensor library | Adafruit | Latest |
| ArduinoJson | Benoit Blanchon | Latest |

> ✅ **No Firebase library needed** — uses built-in `HTTPClient` to avoid SSL issues.

### Board Settings
| Setting | Value |
|---------|-------|
| Board | ESP32 Dev Module |
| Port | COM3 (Windows) or /dev/ttyUSB0 (Linux) |
| Upload Speed | 921600 |
| Driver | Silicon Labs CP2102 |

### Key Config in Code
```cpp
#define WIFI_SSID      "YOUR_WIFI_SSID"
#define WIFI_PASSWORD  "YOUR_WIFI_PASSWORD"
#define DATABASE_URL   "https://XXXXXX-default-rtdb.REGION.firebasedatabase.app"
```

---

## 🎮 Unity Setup

### Project Settings
| Setting | Value |
|---------|-------|
| Unity Version | 6000.3.6f1 |
| Render Pipeline | URP |
| Build Target | Android |
| Package Name | com.yourname.roomdigitaltwin |
| Min API | Android 10 (API 29) |
| Internet Access | Require |
| XR Plugin | OpenXR |

### Scripts Overview

| Script | Role |
|--------|------|
| `LiveData.cs` | ScriptableObject — shared data container for all sensor values |
| `FirebaseReader.cs` | Polls Firebase REST API every 2s via UnityWebRequest |
| `RoomTwin.cs` | Drives visual changes: wall color, light intensity, floor glow |
| `SensorUI.cs` | Updates World Space Canvas TextMeshPro with live readings |

### Scene Hierarchy
```
XR Origin (XR Rig)
RoomManager
  ├── FirebaseReader  (LiveData asset assigned)
  ├── RoomTwin       (LiveData asset assigned)
  └── SensorUI       (LiveData asset assigned)
Room
  ├── Walls[], Floor, Ceiling
  ├── TubeLights[], PointLight
  ├── Furniture (Bed, PC, AC, Cupboard...)
  └── VR_Canvas (World Space)
       ├── Temperature (TextMeshProUGUI)
       ├── Humidity    (TextMeshProUGUI)
       ├── LightData   (TextMeshProUGUI)
       └── MotionDetection (TextMeshProUGUI)
```

### How LiveData Flows
```
FirebaseReader.cs
      │  writes every 2s
      ▼
  LiveData.cs  (ScriptableObject)
      │
      ├──► RoomTwin.cs   → visual reactions
      └──► SensorUI.cs   → UI text updates
```

---

## 🐛 Struggles & Fixes

| # | Problem | Fix |
|---|---------|-----|
| 1 | ESP32 not detected — no COM port | CP2102 driver missing → installed Silicon Labs CP2102 driver |
| 2 | Had to press BOOT button every single upload | Added 10µF capacitor between EN and GND → auto-reset works |
| 3 | Wrong Firebase library causing build errors | "Firebase ESP32 Client" was wrong → installed "Firebase Arduino Client Library for ESP8266 and ESP32" by Mobizt |
| 4 | `ERROR.mConnectSSL` infinite loop with Firebase library | Ditched library entirely → switched to plain `HTTPClient` REST PUT calls |
| 5 | Arduino IDE compile error on `°` and `✓` characters | "Extended character not valid" → replaced with plain text `C` and `!` |
| 6 | PIR always showing DETECTED, never clearing | Default time delay pot = 5–10 min → turned fully anti-clockwise for 3–5s |
| 7 | Lights not reacting to sensor data in Unity | Walls had Standard shader → switched all to URP/Lit shader |
| 8 | Tube light script changing the metal body (Element 0) not the glowing tube | Changed `tube.materials[0]` → `tube.materials[1]` |
| 9 | Covering LDR made room brighter instead of dimmer | Had `1f - lightRatio` inversion by mistake → removed the inversion |

---

## 📁 Project Structure

```
RoomDigitalTwin/
├── Arduino/
│   └── ESP32_Main/
│       └── ESP32_Main.ino       # Main ESP32 sketch
├── Unity/
│   ├── Assets/
│   │   ├── Scripts/
│   │   │   ├── LiveData.cs
│   │   │   ├── FirebaseReader.cs
│   │   │   ├── RoomTwin.cs
│   │   │   └── SensorUI.cs
│   │   ├── Scenes/
│   │   │   └── RoomScene.unity
│   │   └── Materials/
│   ├── Packages/
│   └── ProjectSettings/
├── Docs/
│   ├── architecture.svg
│   ├── wiring-diagram.png
│   └── portfolio.html
├── .gitignore
└── README.md
```

---

## 🚀 Getting Started

### 1. Flash the ESP32
```bash
# Open Arduino/ESP32_Main/ESP32_Main.ino
# Set your WiFi credentials and Firebase URL
# Select Board: ESP32 Dev Module, Port: COM3
# Click Upload
```

### 2. Verify Firebase Stream
Open Firebase Console → Realtime Database. You should see values updating every 2 seconds under `/room/`.

### 3. Open Unity Project
```
Unity Hub → Add Project → select /Unity folder
Switch platform to Android
Set XR Plugin to OpenXR
Assign LiveData asset to all 3 scripts on RoomManager
Build → APK
```

### 4. Sideload to Quest 3
```bash
# Enable Developer Mode on Quest 3
adb install RoomDigitalTwin.apk
# Or use Meta Quest Developer Hub / SideQuest
```

---

## 🧰 Tech Stack

| Layer | Technology |
|-------|-----------|
| Hardware | ESP32-D0WD-V3, DHT11, PIR HC-SR501, LDR |
| Firmware | Arduino IDE, HTTPClient (built-in) |
| Cloud | Firebase Realtime Database (REST API) |
| Engine | Unity 6000.3.6f1, URP |
| XR | OpenXR, Meta XR SDK |
| Language | C++ (Arduino), C# (Unity) |
| Target Device | Meta Quest 3 |

---

## 👨‍💻 Author

**Surya Vijay** — Unity XR Developer  
[LinkedIn](https://linkedin.com/in/suryavjsv/) · [GitHub](https://github.com/suryavjsv)

---

## 📄 License

MIT License — feel free to use, fork, and build on this project.
