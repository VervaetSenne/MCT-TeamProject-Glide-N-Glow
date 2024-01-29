#include <WiFi.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <Adafruit_NeoPixel.h>
#include <ETH.h>
#include "secrets.h"

#define CONFIG_DISABLE_HAL_LOCKS

// Replace the next variables with your SSID/Password combination
const char* ssid = SECRET_SSID;
const char* password = SECRET_PASS;

// Add your MQTT Broker IP address, example:
//const char* mqtt_server = "192.168.1.144";
const char* mqtt_server = MQTT_IP;
#define MQTTsubQos 2 //qos of subscribe

WiFiClient ethClient;
PubSubClient client(ethClient);

#define ETH_POWER_PIN   16

// Type of the Ethernet PHY (LAN8720 or TLK110)
#define ETH_TYPE        ETH_PHY_LAN8720

// I²C-address of Ethernet PHY (0 or 1 for LAN8720, 31 for TLK110)
#define ETH_ADDR        1

// Pin# of the I²C clock signal for the Ethernet PHY
#define ETH_MDC_PIN     23

// Pin# of the I²C IO signal for the Ethernet PHY
#define ETH_MDIO_PIN    18

// MAC address of Device 1
uint8_t mac[] = {0xDE, 0xAD, 0xAE, 0xEF, 0xFE, 0xEE};

// Select the IP address for Device 1
IPAddress localIP(10, 10, 10, 20); // IP address of Device 1

// Select the subnet mask for Device 1
IPAddress subnet(255, 255, 255, 0); // Subnet mask for Device 1


long lastMsg = 0;
char msg[50];
int value = 0;

// NeoPixel Info
int PIN_NEO_PIXEL = 15;  // The ESP32 pin GPIO2 connected to NeoPixel
int NUM_PIXELS = 20;     // The number of LEDs (pixels) on NeoPixel LED strip

Adafruit_NeoPixel PixelStrip = Adafruit_NeoPixel(NUM_PIXELS, PIN_NEO_PIXEL, NEO_GRB + NEO_KHZ800);

const String configTopic = "esp32strip/config";
const String ledTopic = "esp32strip/led";
const String connectedTopic = "esp32strip/connected";
const String updateTopic = "esp32strip/update";
const String ackTopic = "esp32strip/acknowledge";

String clientName = "";

void setup() {
  Serial.begin(115200);
  
  ETH.begin(ETH_ADDR, ETH_POWER_PIN, ETH_MDC_PIN, ETH_MDIO_PIN, ETH_TYPE, ETH_CLK_MODE);
  ETH.macAddress(mac);
  Serial.println(ETH.macAddress());
  ETH.config(localIP, IPAddress(), subnet);
  
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);
  client.setBufferSize(4096);

  clientName = "ESP32"+WiFi.macAddress();
  // Serial.println("Led Topic: " + ledTopic + " " + "Button Topic: " + buttonTopic);
    
  PixelStrip.begin();  // initialize NeoPixel strip object (REQUIRED)
  PixelStrip.setBrightness(122);

  reconnect();
  client.publish(connectedTopic.c_str(), reinterpret_cast<const uint8_t*>("true"), 4, true);
}

void loop() {
  if (!client.connected() && WiFi.status() == 3) {
    reconnect();
  }
  client.loop();
}

void reconnect() {
  // Loop until we're reconnected
  Serial.println("reconnecting...");
  while (!client.connected()) {
    // Serial.print("Attempting MQTT connection...");
    // Attempt to connect
    if (client.connect(clientName.c_str())) {
      Serial.println("connected");
      // Subscribe
      client.subscribe(ledTopic.c_str());
      client.subscribe(ackTopic.c_str());
      client.subscribe(configTopic.c_str());
      client.subscribe(connectedTopic.c_str());
      client.subscribe(updateTopic.c_str());
    } else {
      int state = client.state();
      Serial.print("Connection failed, state=");
      Serial.println(state);

      switch (state) {
        case -4:
          Serial.println("MQTT_CONNECTION_TIMEOUT");
          break;
        case -3:
          Serial.println("MQTT_CONNECTION_LOST");
          break;
        case -2:
          Serial.println("MQTT_CONNECT_FAILED");
          break;
        case -1:
          Serial.println("MQTT_DISCONNECTED");
          break;
        case 1:
          Serial.println("MQTT_CONNECTED");
          break;
        case 2:
          Serial.println("MQTT_CONNECT_BAD_PROTOCOL");
          break;
        case 3:
          Serial.println("MQTT_CONNECT_BAD_CLIENT_ID");
          break;
        case 4:
          Serial.println("MQTT_CONNECT_BAD_CREDENTIALS");
          break;
        case 5:
          Serial.println("MQTT_CONNECT_UNAVAILABLE");
          break;
        case 6:
          Serial.println("MQTT_CONNECT_BAD_WILL_TOPIC");
          break;
        case 7:
          Serial.println("MQTT_CONNECT_BAD_WILL_PAYLOAD");
          break;
        default:
          Serial.println("Unknown error");
          break;
      }
      Serial.println("Retrying in 5 seconds...");
      delay(5000);  // Wait for 5 seconds before retrying
    }
  }
}

void callback(char* topic, byte* message, unsigned int length) {
  Serial.println("Message arrived on topic");

  // Serial.print("Message arrived on topic: ");  
  // Serial.print(topic);
  // Serial.print(". Message: ");

  String MQTTmessage = "";
  for (int i = 0; i < length; i++) {
    // Serial.print((char)message[i]);
    MQTTmessage += (char)message[i];
  }
  // Serial.println(messageLed);
  if (String(topic) == configTopic) {
    // Set config
    Serial.println("config updated");
    NUM_PIXELS = MQTTmessage.toInt();

    Serial.println(NUM_PIXELS);

    PixelStrip.updateLength(NUM_PIXELS);
    client.setBufferSize(NUM_PIXELS * 6 + 32);
  } else if (String(topic) == ackTopic) {
    // Ping pong code
    Serial.println("acknowledge");
    if (MQTTmessage == "ping") {
      client.publish(ackTopic.c_str(), "pong");
    }
  } else if (String(topic) == ledTopic) {
    for (int i = 0; i < MQTTmessage.length(); i += 6){
        uint32_t result = strtoul(MQTTmessage.substring(i, i + 6).c_str(), nullptr, 16);
        PixelStrip.setPixelColor(i / 6, result);
    }
    PixelStrip.show();
}
}