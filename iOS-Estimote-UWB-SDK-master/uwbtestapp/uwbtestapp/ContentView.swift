//
//  ContentView.swift
//  uwbtestapp
//
//  Created by DJ HAYDEN on 1/14/22.
//

import SwiftUI
import UIKit

struct ContentView: View {
  @State private var loading = false
  @ObservedObject private var unity = Unity.shared
  
  let uwb = EstimoteUWBManagerExample.shared
  let bleMonitor = BeaconPeripheralMonitor.shared
  
  var body: some View {
    ZStack{
      if loading {
        // Unity is starting up or shutting down
        ProgressView("Loading...").tint(.white).foregroundStyle(.white)
      } else if let UnityContainer = unity.view.flatMap({ UIViewContainer(containee: $0) }) {
        // Unity is running
        UnityContainer.ignoresSafeArea()
        
      } else {
        VStack{
//          Button("Connect to anchors"){
//            uwb.testConnect()
//          }
//          .padding()
          Button("Start Unity with UWB", systemImage: "play", action: {
            /* Unity startup is slow and must must occur on the
             main thread. Use async dispatch so we can re-render
             with a ProgressView before the UI becomes unresponsive. */
            BeaconPeripheralMonitor.shared.startScan() // Note bad to do this without checking if central is powered on
            loading = true
            DispatchQueue.main.async(execute: {
              unity.start()
              loading = false
            })
          })
          .padding()
          // Unity is not running
          Button("Start Unity without UWB", systemImage: "play", action: {
            /* Unity startup is slow and must must occur on the
             main thread. Use async dispatch so we can re-render
             with a ProgressView before the UI becomes unresponsive. */
            loading = true
            DispatchQueue.main.async(execute: {
              unity.start()
              loading = false
            })
          })
          .padding()
        }
      }
    }
  }
}

struct UIViewContainer: UIViewRepresentable {
    let containee: UIView

    func makeUIView(context: Context) -> UIView {
        return containee
    }

    func updateUIView(_ uiView: UIView, context: Context) {}
}


