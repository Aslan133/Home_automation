#include <SPI.h>
#include <Ethernet.h>
#include <DHT.h>;
 
// Arduino server MAC address
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
// Arduino server IP address
IPAddress ip(192, 168, 0, 111);
//IPAddress ip_server(192, 168, 0, 16);
IPAddress ip_server(192, 168, 0, 100);
// Arduino server port
const int port = 11000;

//Constants
#define DHTPIN 3     // what pin we're connected to
#define DHTTYPE DHT22   // DHT 22  (AM2302)
DHT dht(DHTPIN, DHTTYPE); //// Initialize DHT sensor for normal 16mhz Arduino

float hum;  //Stores humidity value
float temp; //Stores temperature value

EthernetClient client;

// For storing command from client
String commandStr;
String readString;

void setup() {
    // Set digital pin 2 (PD2) as output for LED
    bitSet(DDRD, 2);

    // Ethernet server initialization
    Ethernet.begin(mac, ip);
 
    Serial.begin(9600);
 
    delay(1000);
 
    dht.begin();
}
 
void loop() {
  // Send temp-hum to pc
    hum = dht.readHumidity();
    temp= dht.readTemperature();
    Serial.println(String(temp) + "&" + String(hum));
  
  // Print TCP server client connection info
    if (client.connect(ip_server, port)) {
      Serial.println("connected");
      Serial.println("Sending: " + String(temp) + "&" + String(hum));
      client.println(String(temp) + "&" + String(hum) + "<");
      client.println();


      while(client.available()){
         char c = client.read();
         if (c != '<') {
            // Add received char to string variable 
            commandStr += c;
        } else {
            // Print received command to serial monitor
            Serial.println("Command: " + commandStr);
   
            // Process the received command
            processCommand(commandStr);
   
            // Clear variable for receive next command
            commandStr = "";
        }
      }
      

      if (!client.connected()) {
        Serial.println();
        Serial.println("disconnecting.");
        client.stop();
        //for(;;)
        //  ;
    
    } else {
      Serial.println("connection failed");
    }
  }

delay(3000);
  
}

void processCommand(String cmd) {
    if (cmd == "on") {
        // Turn on LED
        bitSet(PORTD, 2);
    } else if (cmd == "off") {
        // Turn off LED
        bitClear(PORTD, 2);
    }
    
}
