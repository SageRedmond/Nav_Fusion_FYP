import csv

from pydantic import BaseModel
from pydantic.json import pydantic_encoder
import json
import os
import glob

import numpy as np
from datetime import datetime
import math

#region Types
class TriggeredEvent(BaseModel):
    Name: str
    Description: str
    TimeStamp: str
#endregion

#region File Loading
testRunFolderName = "TestRun5/AfterCodeEdit"

def createCSVFile(fileName: str) -> str:
    folderPath = os.path.join(os.getcwd(), testRunFolderName)
    filePath = os.path.join(folderPath, fileName)
    if os.path.exists(filePath):
        return filePath
        # raise FileExistsError("File Exists Already")
    else:
        open(filePath, "x")
        return filePath

def getFilePath(fileNamePattern: str, trialNumber: int) -> str:
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

def loadEventsFromJson(filePath: str):
    if os.path.exists(filePath):
        with open(filePath, 'r') as json_file:
            return [TriggeredEvent(**data) for data in json.load(json_file)]
    else:
        return []
    

#endregion

#region CSV

trialRange = range(4)

for trialNum in trialRange:
    csvFileName = "TriggeredEvents_Trial_" + str(trialNum) + ".csv"
    csvFilePath = createCSVFile(csvFileName)

    triggeredEventsJsonFilePath = getFilePath("TriggeredEvents_*.json", trialNum)
    trigEvents: list[TriggeredEvent] = loadEventsFromJson(triggeredEventsJsonFilePath)
    data = [["Waypoint Number", "AtWaypoint Time", "LeavingWaypoint Time"]]

    eventsArray = np.array(trigEvents).reshape((-1, 2))
    '''
    Shape is now
    [
    [Immersal Sdk, Immersal First],
    [At, leaving],
    [At, leaving],
    [At, leaving],
    ...
    ]
    '''
    print("Trial " + str(trialNum) + ": " + str(eventsArray.shape))

    for index in range(eventsArray.shape[0]):
        if index != 0:
            # print(eventsArray[index])
            waypointNumber = index - 1
            atTime = eventsArray[index][0].TimeStamp
            leavingTime = eventsArray[index][1].TimeStamp
            newData = [waypointNumber, atTime, leavingTime]
            data.append(newData)

    with open(csvFilePath, 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerows(data)

#endregion