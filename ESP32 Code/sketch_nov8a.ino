#include <WiFi.h>
#include <PubSubClient.h>

#include <Adafruit_NeoPixel.h>

// Replace the next variables with your SSID/Password combination
const char* ssid = "Senne Howest Intl";
const char* password = "Howest.Intl";

// Add your MQTT Broker IP address, example:
//const char* mqtt_server = "192.168.1.144";
const char* mqtt_server = "192.168.128.249";

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
char msg[50];
int value = 0;

// NeoPixel Info
#define PIN_NEO_PIXEL 2  // The ESP32 pin GPIO2 connected to NeoPixel
#define NUM_PIXELS 24     // The number of LEDs (pixels) on NeoPixel LED strip

// Button Pin
#define BUTTON_PIN 21


Adafruit_NeoPixel NeoPixel(NUM_PIXELS, PIN_NEO_PIXEL, NEO_GRB + NEO_KHZ800);

String connectedTopic = "";
String ledTopic = "";
String buttonTopic = "";
int previousButtonState;

String messageLed;

void setup() {
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);

  // Set BUTTON_PIN as an input with pullup resistor
  pinMode(BUTTON_PIN, INPUT_PULLUP);

  connectedTopic = "esp32/"+WiFi.macAddress()+"/connected";
  ledTopic = "esp32/"+WiFi.macAddress()+"/ledcircle";
  buttonTopic = "esp32/"+WiFi.macAddress()+"/button/value";
  // Serial.println("Led Topic: " + ledTopic + " " + "Button Topic: " + buttonTopic);
    
  NeoPixel.begin();  // initialize NeoPixel strip object (REQUIRED)

  reconnect();
  client.publish(connectedTopic.c_str(), "true");
}

void loop() {
  if (!client.connected() && WiFi.status() == 3) {
    reconnect();
  }
  client.loop();
  // read the state of the switch/button:
  int buttonState = digitalRead(BUTTON_PIN);
  // char buttonMessage[buttonState];
  // Serial.println(buttonMessage);
  if (buttonState == LOW && buttonState != previousButtonState){
    Serial.println("pressed");
    client.publish(buttonTopic.c_str(), "true");
  }
  delay(100);
  previousButtonState = buttonState;
  // light_up_circle_animated(255,0,0);
  // light_up_circle(0,0,255);
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

void light_up_circle_animated(int red,int green, int blue) {
  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {                  // for each pixel
    NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));  // it only takes effect if pixels.show() is called
    NeoPixel.show();                                                  // update to the NeoPixel Led Strip

    delay(50);  // 50ms pause between each pixel
  }
}

void light_up_circle(int red,int green, int blue) {
  for (int pixel = 0; pixel < NUM_PIXELS; pixel++) {                  // for each pixel
    NeoPixel.setPixelColor(pixel, NeoPixel.Color(red, green, blue));  // it only takes effect if pixels.show() is called
  }
  NeoPixel.show();         // update to the NeoPixel Led Strip
}

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) {
    Serial.print("Attempting MQTT connection...");
    // Attempt to connect
    if (client.connect("ESP8266Client")) {
      Serial.println("connected");
      // Subscribe
      client.subscribe(ledTopic.c_str());
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
}

void callback(char* topic, byte* message, unsigned int length) {
  Serial.print("Message arrived on topic: ");
  Serial.print(topic);
  Serial.print(". Message: ");
  
  for (int i = 0; i < length; i++) {
    // Serial.print((char)message[i]);
    messageLed += (char)message[i];
  }

  if (String(topic) == ledTopic) {
    if(messageLed == "on"){
      Serial.println("on");
      light_up_circle(0,0,255);
    }
    else if(messageLed == "off"){
      Serial.println("off");
      light_up_circle(0,0,0);
    }
  }
}