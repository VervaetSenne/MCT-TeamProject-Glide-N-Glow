#include <WiFi.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <Adafruit_NeoPixel.h>
#include "secrets.h"

// Replace the next variables with your SSID/Password combination
const char* ssid = SECRET_SSID;
const char* password = SECRET_PASS;

// Add your MQTT Broker IP address, example:
//const char* mqtt_server = "192.168.1.144";
const char* mqtt_server = MQTT_IP;
#define MQTTsubQos 2 //qos of subscribe

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;

// NeoPixel Info
#define PIN_NEO_PIXEL 32  // The ESP32 pin GPIO2 connected to NeoPixel
#define NUM_PIXELS 16     // The number of LEDs (pixels) on NeoPixel LED strip
Adafruit_NeoPixel NeoPixel(NUM_PIXELS, PIN_NEO_PIXEL, NEO_GRB + NEO_KHZ800);

// Button Pin
#define BUTTON_PIN 26

bool ledOn;
bool previousLedOn;

String connectedTopic = "";
String ledTopic = "";
String buttonTopic = "";
String ackTopic = "";

String clientName = "";
int previousButtonState;

int r = 0;
int g = 0;
int b = 0;
int ani = 0;

void setup() {
  Serial.begin(115200);
  setup_wifi();
  Serial.print("test1");
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);
  Serial.print("test2");

  // Set BUTTON_PIN as an input with pullup resistor
  pinMode(BUTTON_PIN, INPUT_PULLUP);

  connectedTopic = "esp32/"+WiFi.macAddress()+"/connected";
  
  Serial.print("test1");
  ledTopic = "esp32/"+WiFi.macAddress()+"/ledcircle";
  buttonTopic = "esp32/"+WiFi.macAddress()+"/button";
  ackTopic = "esp32/"+WiFi.macAddress()+"/acknowledge";
  
  Serial.print("test1");
  clientName = "ESP32"+WiFi.macAddress();

  Serial.println("Led Topic: " + ledTopic + " " + "Button Topic: " + buttonTopic);
    
  NeoPixel.begin();  // initialize NeoPixel strip object (REQUIRED)
  reconnect();
  client.publish(connectedTopic.c_str(), reinterpret_cast<const uint8_t*>("true"), 4, true);
}

void loop() {
  if (!client.connected() && WiFi.status() == 3) {
    reconnect();
  }
  if (WiFi.status() != WL_CONNECTED) {
    reconnect_wifi();
  }
  client.loop();
  // read the state of the switch/button:
  int buttonState = digitalRead(BUTTON_PIN);
  // char buttonMessage[buttonState];
  // Serial.println(buttonMessage);
  if (buttonState == LOW && buttonState != previousButtonState){
    Serial.println("pressed");
    client.publish(buttonTopic.c_str(), reinterpret_cast<const uint8_t*>("true"), 4, true);
  }
  delay(100);
  previousButtonState = buttonState;
}

void setup_wifi() {
  delay(10);
  // We start by connecting to a WiFi network
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  Serial.println(WiFi.macAddress());
}

void reconnect_wifi(){
  Serial.println("Reconnecting to WiFi...");
  WiFi.disconnect();
  WiFi.reconnect();
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
}

void light_up_circle(int red, int green, int blue) {
  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {                  // for each pixel
    NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));  // it only takes effect if pixels.show() is called
  }
  NeoPixel.show();// update to the NeoPixel Led Strip
}

void light_up_circle_animated(int red, int green, int blue) {
  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {                  // for each pixel
    NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));  // it only takes effect if pixels.show() is called
    NeoPixel.show();                                                  // update to the NeoPixel Led Strip

    delay(50);  // 50ms pause between each pixel
  }
}

void light_up_circle_animated1(int red, int green, int blue) {
  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {
    if (pixel % 2 == 0) {
      NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));
      NeoPixel.show();
      delay(50);
    }
  }

  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {
    if (pixel % 2 == 1) {
      NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));
      NeoPixel.show();
      delay(50);
    }
  }
}

void light_up_circle_animated2(int red, int green, int blue) {
  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {
    if (pixel % 3 == 0) {
      NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));
      NeoPixel.show();
      delay(50);
    }
  }

  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {
    if (pixel % 3 == 1) {
      NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));
      NeoPixel.show();
      delay(50);
    }
  }

  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {
    if (pixel % 3 == 2) {
      NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));
      NeoPixel.show();
      delay(50);
    }
  }
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
    } else {
      int state = client.state();
      Serial.print("Connection failed, state=");
      Serial.println(state);

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
  
  if (String(topic) == ackTopic) {
    Serial.println("acknowledge");
    if(MQTTmessage == "ping"){
      client.publish(ackTopic.c_str(), "pong");
    }
  }
  else if (String(topic) == ledTopic) {
    StaticJsonDocument<64> doc;
    DeserializationError error = deserializeJson(doc, message);

    if (error) {
    Serial.print("deserializeJson() failed: ");
    Serial.println(error.c_str());
    return;
    }
  r = doc["r"];
  g = doc["g"];
  b = doc["b"];
  ani = doc["ani"];
  switch(ani){
    case 1:
      light_up_circle(r,g,b);
      break;
    case 2:
      light_up_circle_animated(r,g,b);
      break;
    case 3:
      light_up_circle_animated1(r,g,b);
      break;
    case 4:
      light_up_circle_animated2(r,g,b);
      break;
    default:
      light_up_circle_animated(r,g,b);
      break;
    }
  }
}