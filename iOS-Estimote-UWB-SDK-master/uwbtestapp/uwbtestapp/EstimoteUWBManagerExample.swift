//
//  EstimoteUWBManagerExample.swift
//  uwbtestapp
//
//  Created by Sage Redmond on 05/07/2025.
//

import SwiftUI
import EstimoteUWB
import OSLog

struct DiscoveredBeacon: Identifiable{
  var id: String
  var device: CBPeripheral
  var rssi: Float
}

class EstimoteUWBManagerExample: NSObject, ObservableObject {
  public static let shared = EstimoteUWBManagerExample()
  
  private var connectedBeaconId: String = ""
  private var unity = Unity.shared
  private var uwbManager: EstimoteUWBManager?
  
  private var discoveredBeacons: [DiscoveredBeacon] = []
  
  private var shouldHandleConnectivity: Bool = false
  let logger = Logger()
  
  private override init() {
    super.init()
    setupUWB()
  }
  
  private func setupUWB() {
    uwbManager = EstimoteUWBManager(delegate: self,
                                    options: EstimoteUWBOptions(shouldHandleConnectivity: false,
                                                                isCameraAssisted: false))
    uwbManager?.startScanning()
  }
  
  func refreshList(){
    discoveredBeacons.removeAll()
    self.uwbManager?.stopScanning()
    
    DispatchQueue.main.asyncAfter(deadline: .now() + 1.0){
      self.uwbManager?.startScanning()
    }
  }
  
//  func connectToAnchorWithHighestRSSI(){
//    discoveredBeacons.sort(by: {$0.rssi > $1.rssi})
//    if let beacon = discoveredBeacons.first{
//      let beaconID = beacon.id
//      let beaconRSSI = beacon.rssi
//      
//      if beaconID != connectedBeaconId{
//        if beaconRSSI >= -70.0 {
//          let previousConnection = connectedBeaconId
//          if previousConnection != "" {
//            logger.warning("Calling Disconnect on \(previousConnection)")
//            uwbManager?.disconnect(from: previousConnection)
////            BeaconPeripheralMonitor.shared.disconnectPeripheral(device: beacon.device)
//          }
//          logger.info("Connecting to beacon: \(beaconID)")
//          uwbManager?.connect(to: beaconID)
//        }
//      }
//    }
//    
//    
//  }
  func testConnect(){
    uwbManager?.connect(to: "450ed09104d134339be51d3cd5f8ef3c")
  }
  
  func connectToBeaconsWithHighRSSI(){
    for (index, beacon) in discoveredBeacons.enumerated() {
        if beacon.rssi >= -90.0 {
          uwbManager?.connect(to: beacon.id)
          discoveredBeacons.remove(at: index)
        }
    }
  }
  
  func updateDiscoveredBeacon(beaconId: String, scannedDevice: CBPeripheral, rssi: Float){
    if let index = discoveredBeacons.firstIndex(where: { $0.id == beaconId }) {
        discoveredBeacons[index].rssi = rssi
    }
    else{
      let beacon = DiscoveredBeacon(id: beaconId, device: scannedDevice, rssi: rssi)
      discoveredBeacons.append(beacon)
    }
    connectToBeaconsWithHighRSSI()
  }
}

// REQUIRED PROTOCOL
extension EstimoteUWBManagerExample: EstimoteUWBManagerDelegate {
  func didUpdatePosition(for device: EstimoteUWBDevice) {
//    self.logger.info("Position updated for device \(device.id): \(device.distance)")
    
    DispatchQueue.main.async{
      self.unity.setBeaconData(beaconId: device.id, range: device.distance)
//      self.uwbManager?.disconnect(from: device.publicIdentifier)
    }
  }
  
  // OPTIONAL
  func didDiscover(device: UWBIdentifiable, with rssi: NSNumber, from manager: EstimoteUWBManager) {
    print("Discovered device: \(device.publicIdentifier) rssi: \(rssi)")
    // if shouldHandleConnectivity is set to true - then you could call manager.connect(to: device)
    // additionally you can globally call discoonect from the scope where you have inititated EstimoteUWBManager -> disconnect(from: device) or disconnect(from: publicId)

//    discoveredBeacons.removeAll(where: {$0.id == device.publicIdentifier})
//    let beacon = DiscoveredBeacon(id: device.publicIdentifier, rssi: rssi.floatValue)
//    discoveredBeacons.append(beacon)
  }
  
  // OPTIONAL
  func didConnect(to device: UWBIdentifiable) {
    print("Successfully connected to: \(device.publicIdentifier)")
//    DispatchQueue.main.async{
//      self.connectedBeaconId = device.publicIdentifier
//    }
    connectedBeaconId = device.publicIdentifier
  }
  
  // OPTIONAL
  func didDisconnect(from device: UWBIdentifiable, error: Error?) {
    print("Disconnected from device: \(device.publicIdentifier)- error: \(String(describing: error))")
  }
  
  // OPTIONAL
  func didFailToConnect(to device: UWBIdentifiable, error: Error?) {
    print("Failed to conenct to: \(device.publicIdentifier) - error: \(String(describing: error))")
  }
  
  // OPTIONAL PROTOCOL FOR BEACON BLE RANGING
//      func didRange(for beacon: EstimoteBLEDevice) {
//          print("Beacon did range: \(beacon)")
//      }
}
