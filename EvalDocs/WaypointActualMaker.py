import csv

from pydantic import BaseModel
from pydantic.json import pydantic_encoder
import json
import os
import glob

import numpy as np
from datetime import datetime
import math

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

csvFileName = "WaypointsActual.csv"
csvFilePath = createCSVFile(csvFileName)

waypoints = [["X", "Y", "Z"]]

for index in range(10):
    # Choosing Z as the forward direction
    Z = 2.50 * index
    waypoints.append([0.0, 0.0, Z])

with open(csvFilePath, 'w', newline='') as csvfile:
    writer = csv.writer(csvfile)
    writer.writerows(waypoints)
