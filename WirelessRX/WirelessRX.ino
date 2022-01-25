#include <PPMReader.h>
// #include <InterruptHandler.h>   <-- You may need this on some versions of Arduino

// Initialize a PPMReader on digital pin 3 with 6 expected channels.
int interruptPin = 2;
int channelAmount = 8;
PPMReader ppm(interruptPin, channelAmount);

void setup() {
    Serial.begin(115200);
}

void loop() {
  unsigned long ch1 = ppm.latestValidChannelValue(1, 0);
  unsigned long ch2 = ppm.latestValidChannelValue(2, 0);
  unsigned long ch3 = ppm.latestValidChannelValue(3, 0);
  unsigned long ch4 = ppm.latestValidChannelValue(4, 0);

  unsigned long sw1 = ppm.latestValidChannelValue(5, 0);
  unsigned long sw2 = ppm.latestValidChannelValue(7, 0);

  if (ch1 + ch4 > 3990 && ch3 + ch2 < 2010) {
      sw2 = 2000;
  }
  
  if (ch1 + ch4 < 2010 && ch3 + ch2 < 2010) {
      sw1 = 2000;
  }
  
      Serial.println("A6N " + String(ch1) + " " + String(ch2) + " " + String(ch3) + " " + String(ch4) + " " + String(sw2) + " " + String(sw1));
}
