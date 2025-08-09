//
//  EstimoteUWBManagerExample.swift
//  uwbtestapp
//
//  Created by Sage Redmond on 05/07/2025.
//

import SwiftUI
import EstimoteUWB

class EstimoteUWBManagerExample: NSObject, ObservableObject {
  @Published var distance: Float = 0.0
//  let beaconID = "d04567bc3557ff70ca197e3c8c236119"
  let beaconID = "70f0576ae14090a92231974cccec402d"
  private var unity = Unity.shared
  private var uwbManager: EstimoteUWBManager?
  
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
}

// REQUIRED PROTOCOL
extension EstimoteUWBManagerExample: EstimoteUWBManagerDelegate {
  func didUpdatePosition(for device: EstimoteUWBDevice) {
    print("Position updated for device: \(device)")
    
    DispatchQueue.main.async{
      self.distance =  device.distance
      self.unity.setDistance(to: device.distance)
      if let direction = device.vector{
        self.unity.setDirection(to: direction)
      }
      else{
        self.unity.setNoDirection()
      }
    }
  }
  
  // OPTIONAL
  func didDiscover(device: UWBIdentifiable, with rssi: NSNumber, from manager: EstimoteUWBManager) {
    print("Discovered device: \(device.publicIdentifier) rssi: \(rssi)")
    // if shouldHandleConnectivity is set to true - then you could call manager.connect(to: device)
    // additionally you can globally call discoonect from the scope where you have inititated EstimoteUWBManager -> disconnect(from: device) or disconnect(from: publicId)
    
    if device.publicIdentifier == self.beaconID{
      uwbManager?.connect(to: device)
    }
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
  //    func didRange(for beacon: EstimoteBLEDevice) {
  //        print("Beacon did range: \(beacon)")
  //    }
}
