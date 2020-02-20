#include <SPI.h>
#include <Ethernet.h>
 
// Arduino server MAC address
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
// Arduino server IP address
IPAddress ip(192, 168, 0, 10);
// Arduino server port
const int port = 23;
 
EthernetServer server(port);
// For storing command from client
String commandStr;
String readString;

void setup() {
    // Set digital pin 2 (PD2) as output for LED
    bitSet(DDRD, 2);
 
    Serial.begin(9600);
 
    // Ethernet server initialization
    Ethernet.begin(mac, ip);
    server.begin();
 
    // Print Arduino server IP address to serial monitor
    Serial.print("Server is at ");
    Serial.println(Ethernet.localIP());
}
 
void loop() {
    EthernetClient client = server.available();

    if (client.available()) {
        // Read char until linefeed
        char c = client.read(); 
        if (c != '0') {
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
}
void processCommand(String cmd) {
    if (cmd == "on") {
        // Turn on LED
        bitSet(PORTD, 2);
        server.println(cmd);
    } else if (cmd == "off") {
        // Turn off LED
        bitClear(PORTD, 2);
        server.println(cmd);
    }
}
