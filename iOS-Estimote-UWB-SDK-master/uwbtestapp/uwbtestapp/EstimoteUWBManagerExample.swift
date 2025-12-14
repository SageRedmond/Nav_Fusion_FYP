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
  var rssi: Float
}

class EstimoteUWBManagerExample: NSObject, ObservableObject {
  @Published var connectedBeaconId: String = ""
  @Published var distance: Float = 0.0
  
  private var unity = Unity.shared
  private var uwbManager: EstimoteUWBManager?
  
  private var discoveredBeacons: [DiscoveredBeacon] = []
  
  let logger = Logger()
  override init() {
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
  
  func connectToAnchorWithHighestRSSI(){
    let anchorID = discoveredBeacons.sorted {$0.rssi > $1.rssi}.first?.id
    print(anchorID ?? "No Id")
    if anchorID != nil{
      uwbManager?.connect(to: anchorID!)
    }
  }
}

// REQUIRED PROTOCOL
extension EstimoteUWBManagerExample: EstimoteUWBManagerDelegate {
  func didUpdatePosition(for device: EstimoteUWBDevice) {
    self.logger.info("Position updated for device: \(device)")
    
    DispatchQueue.main.async{
      self.unity.setBeaconData(beaconId: device.id, range: device.distance)
    }
  }
  
  // OPTIONAL
  func didDiscover(device: UWBIdentifiable, with rssi: NSNumber, from manager: EstimoteUWBManager) {
    print("Discovered device: \(device.publicIdentifier) rssi: \(rssi)")
    // if shouldHandleConnectivity is set to true - then you could call manager.connect(to: device)
    // additionally you can globally call discoonect from the scope where you have inititated EstimoteUWBManager -> disconnect(from: device) or disconnect(from: publicId)

    discoveredBeacons.removeAll(where: {$0.id == device.publicIdentifier})
    let beacon = DiscoveredBeacon(id: device.publicIdentifier, rssi: rssi.floatValue)
    discoveredBeacons.append(beacon)
  }
  
  // OPTIONAL
  func didConnect(to device: UWBIdentifiable) {
    print("Successfully connected to: \(device.publicIdentifier)")
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
