import pandas as pd
import os
import glob
import math
import csv

testRunFolderName = "TestRun5/AfterCodeEdit"
coordFileType = "Unity"

NUM_WAYPOINTS = 10

def get3dPointDistance(pose1, pose2) -> float:
    x = math.pow((pose1[0] - pose2[0]), 2)
    y = math.pow((pose1[1] - pose2[1]), 2)
    z = math.pow((pose1[2] - pose2[2]), 2)
    distance = math.sqrt(x + y + z)
    return distance

def get2DPointDistance(pose1, pose2) -> float:
    x = math.pow((pose1[0] - pose2[0]), 2)
    z = math.pow((pose1[2] - pose2[2]), 2)
    distance = math.sqrt(x + z)
    return distance

def getFilePath(fileNamePattern: str, trialNumber = 0) -> str:
    folderPath = os.path.join(os.getcwd(), testRunFolderName)
    pattern = os.path.join(folderPath, fileNamePattern)
    # file_path = os.path.join(folderPath, file_name)
    matching_files = glob.glob(pattern)
    matching_files.sort()

    if matching_files:
        file_path = matching_files[trialNumber]
    else:
        raise FileNotFoundError("No file found")
    
    return file_path

def createCSVFile(fileName: str) -> str:
    folderPath = os.path.join(os.getcwd(), testRunFolderName)
    filePath = os.path.join(folderPath, fileName)
    if os.path.exists(filePath):
        # raise FileExistsError("File Exists Already")
        return filePath
    else:
        open(filePath, "x")
        return filePath

def filterWaypointPoses(poses, timestamps):
    waypointPoses = []
    for rowNum in range(10):
        AtWaypoint1 = timestamps["AtWaypoint Time"].loc[timestamps.index[rowNum]]
        LeavingWaypoint1 = timestamps["LeavingWaypoint Time"].loc[timestamps.index[rowNum]]
        mask = (poses["Timestamp"] >= AtWaypoint1) & (poses["Timestamp"] <= LeavingWaypoint1)

        poseData_df = poses[mask]

        waypointPoses.append(poseData_df)
    
    return waypointPoses

def getTrialWaypointCentroids(waypointPoses):
    waypointCentroids = []

    for wpData_df in waypointPoses:
        # centroid = [wpData_df["X"].mean(), wpData_df["Y"].mean(), wpData_df["Z"].mean()]
        # # print("%.3f , %.3f, %.3f" % (centroid[0], centroid[1], centroid[2]))
        # waypointCentroids.append(centroid)
        waypointCentroids.append(wpData_df["X"].mean())
        waypointCentroids.append(wpData_df["Y"].mean())
        waypointCentroids.append(wpData_df["Z"].mean())


    return waypointCentroids


def main():
    numberOfTrials = 4

    data = [["WPC0_X", "WPC0_Y", "WPC0_Z", 
             "WPC1_X", "WPC1_Y", "WPC1_Z", 
             "WPC2_X", "WPC2_Y", "WPC2_Z", 
             "WPC3_X", "WPC3_Y", "WPC3_Z",
             "WPC4_X", "WPC4_Y", "WPC4_Z", 
             "WPC5_X", "WPC5_Y", "WPC5_Z", 
             "WPC6_X", "WPC6_Y", "WPC6_Z", 
             "WPC7_X", "WPC7_Y", "WPC7_Z", 
             "WPC8_X", "WPC8_Y", "WPC8_Z", 
             "WPC9_X", "WPC9_Y", "WPC9_Z"]]
    gatheredDataCsvFileName = "Gathered_Data_" + str(0) + ".csv"
    gatheredDataCsvFilePath = createCSVFile(gatheredDataCsvFileName)

    for trialNum in range(numberOfTrials):
        csvFileName = coordFileType + "Coordinates_Trial_" + str(trialNum) + ".csv"
        csvFilePath = getFilePath(csvFileName)

        triggeredEventsFileName = "TriggeredEvents_Trial_" + str(trialNum) + ".csv"
        triggeredEventsFilePath = getFilePath(triggeredEventsFileName)

        pose_df = pd.read_csv(csvFilePath)

        timeStamp_df = pd.read_csv(triggeredEventsFilePath)

        filteredWaypointPoses = filterWaypointPoses(poses=pose_df, timestamps=timeStamp_df)

        waypointCentroids = getTrialWaypointCentroids(filteredWaypointPoses)
        # print(waypointCentroids)
        data.append(waypointCentroids)
    
    with open(gatheredDataCsvFilePath, 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerows(data)


if __name__=="__main__":
    main()