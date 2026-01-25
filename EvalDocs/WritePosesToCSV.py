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
class Coordinate(BaseModel):
    X: float
    Y: float
    Z: float
    TimeStamp: str

#endregion

#region File Loading
testRunFolderName = "TestRun5/AfterCodeEdit"

def createCSVFile(fileName: str) -> str:
    folderPath = os.path.join(os.getcwd(), testRunFolderName)
    filePath = os.path.join(folderPath, fileName)
    if os.path.exists(filePath):
        # raise FileExistsError("File Exists Already")
        return filePath
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

def loadCoordinatesFromJson(filePath: str):
    if os.path.exists(filePath):
        with open(filePath, 'r') as json_file:
            return [Coordinate(**data) for data in json.load(json_file)]
    else:
        return []
#endregion

#region Write CSV
# trialNum = 3
trialRange = range(4)
# XR for IMU + Optical-Flow poses
# Unity for Map poses
coordFileType = "Unity"

for trialNum in trialRange:
    csvFileName = coordFileType + "Coordinates_Trial_" + str(trialNum) + ".csv"
    csvFilePath = createCSVFile(csvFileName)

    coordsJsonFilePath = getFilePath(coordFileType + "Coordinates_*.json", trialNum)
    listCoords: list[Coordinate] = loadCoordinatesFromJson(coordsJsonFilePath)
    data = [["X", "Y", "Z", "Timestamp"]]

    for coord in listCoords:
        data.append([coord.X, coord.Y, coord.Z, coord.TimeStamp])

    with open(csvFilePath, 'w', newline='') as csvfile:
        writer = csv.writer(csvfile)
        writer.writerows(data)

#endregion