//
//  BeaconPeripheralMonitor.swift
//  uwbtestapp
//
//  Created by Sage Redmond on 14/12/2025.
//

import EstimoteUWB
import OSLog
import CoreBluetooth

final class BeaconPeripheralMonitor: NSObject, ObservableObject{
  public static let shared = BeaconPeripheralMonitor()
  let EstimoteProximityBeaconService = CBUUID(string: "FE9A")
  
  private let logger = Logger()
  private var centralManager: CBCentralManager!
  let uwb = EstimoteUWBManagerExample.shared
  
  private override init(){
    super.init()
    print("Starting BeaconPeripheralMonitor")
    centralManager = CBCentralManager(delegate: self, queue: nil)
  }
  
  //  var discoveredPeripherals = [CBPeripheral]()
  func startScan() {
    print("Starting Scan")
    // https://www.bluetooth.com/wp-content/uploads/Files/Specification/HTML/Assigned_Numbers/out/en/Assigned_Numbers.pdf
    
    // passed in option allows continues monitering of RSSI => Higher power consumption
    centralManager.scanForPeripherals(withServices: [EstimoteProximityBeaconService],
                                      options: [CBCentralManagerScanOptionAllowDuplicatesKey: true])
    //    centralManager.scanForPeripherals(withServices: nil,
    //                                      options: nil)
    //    centralManager.scanForPeripherals(withServices: [EstimoteProximityBeaconService],
    //                                      options: nil)
  }
  
  public func disconnectPeripheral(device: CBPeripheral){
    centralManager.cancelPeripheralConnection(device)
  }
}

extension BeaconPeripheralMonitor: CBCentralManagerDelegate{
  func centralManagerDidUpdateState(_ central: CBCentralManager) {
    switch central.state {
    case .poweredOn:
      startScan()
      print("Powered On")
    case .poweredOff:
      // Alert user to turn on Bluetooth
      print("Powered Off")
    case .resetting:
      // Wait for next state update and consider logging interruption of Bluetooth service
      print("resetting")
    case .unauthorized:
      // Alert user to enable Bluetooth permission in app Settings
      print("unauthorized")
    case .unsupported:
      // Alert user their device does not support Bluetooth and app will not work as expected
      print("unsupported")
    case .unknown:
      // Wait for next state update
      print("unknown")
    @unknown default:
      fatalError()
    }
  }
  
  func centralManager(_ central: CBCentralManager, didDiscover peripheral: CBPeripheral, advertisementData: [String : Any], rssi RSSI: NSNumber) {
    //    print("Name: \(peripheral.name ?? "No Name")")
    //    print("Public Identifier: \(peripheral.publicIdentifier)")
    //    print("Rssi value: \(RSSI.floatValue)")
    //    print(peripheral.services?.debugDescription ?? "No Services")
    //    print("\(advertisementData[CBAdvertisementDataServiceDataKey] ?? "or not")")`
    //    logger.info("Name: \(peripheral.name ?? "No Name") \nPublic Identifier: \(peripheral.publicIdentifier) \nRssi value: \(RSSI.floatValue) \nService Data: \(String(describing: advertisementData[CBAdvertisementDataServiceDataKey] ?? "or not"))")
    let serviceDataDictionary = advertisementData[CBAdvertisementDataServiceDataKey] as? [CBUUID: NSData]
    let beaconIdData = serviceDataDictionary![self.EstimoteProximityBeaconService]
    let trimmedData = beaconIdData!.dropFirst().dropLast(2)
    let uwbBeaconId = trimmedData.map { String(format: "%02x", $0) }.joined()
    //    logger.info("Data: \(String(describing: serviceData![self.EstimoteProximityBeaconService]))")
//    logger.info("Beacon ID: \(uwbBeaconId), Rssi value: \(RSSI.floatValue)")
    uwb.updateDiscoveredBeacon(beaconId: uwbBeaconId, scannedDevice: peripheral, rssi: RSSI.floatValue)
  }
  
  func centralManager(_ central: CBCentralManager, didConnect peripheral: CBPeripheral) {
    print("Peripheral \(peripheral.name ?? "No Name") connected")
  }
}
