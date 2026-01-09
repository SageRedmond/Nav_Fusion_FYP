import rerun as rr

from pydantic import BaseModel
from pydantic.json import pydantic_encoder
import json
import os

import numpy as np
from datetime import datetime
import math

# region Types
class Beacon(BaseModel):
    beaconId: str
    roomId: str
    xpos: float
    ypos: float
    zpos: float

class BeaconsModel(BaseModel):
    beacons: list[Beacon] = []

class Coordinate(BaseModel):
    X: float
    Y: float
    Z: float
    TimeStamp: str

class CoordinateModel(BaseModel):
    coordinates: list[Coordinate] = []
# endregion

#region File Loading
def makeFilePath(testFileName: str) -> str:
    folderPath = os.getcwd()
    os.makedirs(folderPath, exist_ok=True)
    file_name = "TestRun1/" + testFileName
    file_path = os.path.join(folderPath, file_name)
    return file_path

xrCoordinatesJSONFilePath = makeFilePath("XRCoordinates.json")
unityCoordinatesJSONFilePath = makeFilePath("UnityCoordinates.json")
beaconsJsonFilePath = makeFilePath("BeaconList.json")

def loadCoordsFromJson(filePath: str) -> list[Coordinate]:
    if os.path.exists(filePath):
        with open(filePath, 'r') as json_file:
            return [Coordinate(**data) for data in json.load(json_file)]
    else:
        return []
    

def loadBeaconsFromJson(filePath: str):
    if os.path.exists(filePath):
        with open(filePath, 'r') as json_file:
            return [Beacon(**data) for data in json.load(json_file)]
    else:
        return []

xrCoordinates: list[Coordinate] = loadCoordsFromJson(xrCoordinatesJSONFilePath)
unityCoordinates: list[Coordinate] = loadCoordsFromJson(unityCoordinatesJSONFilePath)
beacons: list[Beacon] = loadBeaconsFromJson(beaconsJsonFilePath)

# endregion

# region Util
def switchHandness(pnt: list[float]) -> list[float]:
    angle = math.pi / 2
    # beh = [cr.X, cr.Y, cr.Z]
    # point = np.array([cr.X, cr.Y, cr.Z])
    point = np.array(pnt)
    rotationMtx = np.array([[1.0, 0.0, 0.0], [0.0, math.cos(angle), (-1.0 * math.sin(angle))], [0.0, math.sin(angle), math.cos(angle)]])
    invertYMtx = np.array([[1.0, 0.0, 0.0], [0.0, -1.0, 0.0], [0.0, 0.0, 1.0]])

    result1 = np.matmul(rotationMtx, point)
    finalResult = np.matmul(result1, invertYMtx)
    # finalResult = np.matmul(point, invertYMtx)
    return finalResult.tolist()
    # return result1.tolist()
    # invertZMtx = np.array([[1.0, 0.0, 0.0], [0.0, 1.0, 0.0], [0.0, 0.0, -1.0]])
    # return (np.matmul(result1, invertZMtx)).tolist()

def switchHandnessXR(pnt: list[float]) -> list[float]:
    angle = math.pi / 2
    # beh = [cr.X, cr.Y, cr.Z]
    # point = np.array([cr.X, cr.Y, cr.Z])
    point = np.array(pnt)
    rotationMtx = np.array([[1.0, 0.0, 0.0], [0.0, math.cos(angle), (-1.0 * math.sin(angle))], [0.0, math.sin(angle), math.cos(angle)]])

    result1 = np.matmul(rotationMtx, point)
    # finalResult = np.matmul(result1, invertYMtx)
    # finalResult = np.matmul(point, invertYMtx)
    # return finalResult.tolist()
    return result1.tolist()
    # invertZMtx = np.array([[1.0, 0.0, 0.0], [0.0, 1.0, 0.0], [0.0, 0.0, -1.0]])
    # return (np.matmul(result1, invertZMtx)).tolist()

# endregion

def main():
    # coordinates = loadFromJson()
    for index in range(5):
        cr = xrCoordinates[index]
        print("%.2f , %.2f, %.2f" % (cr.X, cr.Y, cr.Z))
        print(cr.TimeStamp)

# if __name__ == "__main__":
#     main()

#region Rerun
rr.init("rerun_pose_visual", spawn=True)
rr.set_time("stable_time", duration=0)


rr.log(
    "xyz",
    rr.Arrows3D(
        vectors=[[1, 0, 0], [0, 1, 0], [0, 0, 1]],
        colors=[[255, 0, 0], [0, 255, 0], [0, 0, 255]],
    ),
    static=True
)

# region XR Points
xrPointsArray = []
for cr in xrCoordinates:
    # xrPointsArray.append(switchHandness([cr.X, cr.Y, cr.Z]))
    xrPointsArray.append(switchHandnessXR([cr.X, cr.Y, (-1.0 * cr.Z)]))
    # xrPointsArray.append([cr.X, cr.Y, (-1.0 * cr.Z)])
    points = np.array(xrPointsArray)
    rr.set_time("time", timestamp=datetime.fromisoformat(cr.TimeStamp))
    rr.log("paths/xrPath/points", rr.Points3D(points, radii=0.08))

xr2PointsArray = []
for cr in xrCoordinates:
    # xr2PointsArray.append(switchHandness([cr.X, cr.Y, cr.Z]))
    # xr2PointsArray.append(switchHandnessXR([cr.X, cr.Y, (-1.0 * cr.Z)]))
    # xr2PointsArray.append([cr.X, (-1.0 * cr.Z), cr.Y])
    xr2PointsArray.append([cr.X, cr.Y, (-1.0 * cr.Z)])
    points = np.array(xr2PointsArray)
    rr.set_time("time", timestamp=datetime.fromisoformat(cr.TimeStamp))
    rr.log("paths/xrPath2/points", rr.Points3D(points, radii=0.08))
# endregion

# region Unity Points
unityPointsArray = []
for cr in unityCoordinates:
    unityPointsArray.append(switchHandness([cr.X, cr.Y, cr.Z]))
    points = np.array(unityPointsArray)
    rr.set_time("time", timestamp=datetime.fromisoformat(cr.TimeStamp))
    rr.log("paths/unityPath/points", rr.Points3D(points, colors=[[255, 0, 0]], radii=0.08))
# endregion

# region SVD Align
def rigid_transform_3D(A, B):
    """
    Find rotation R and translation t such that B ≈ R @ A + t
    A, B: (N, 3) numpy arrays
    """
    # Center the points
    centroid_A = np.mean(A, axis=0)
    centroid_B = np.mean(B, axis=0)
    
    A_centered = A - centroid_A
    B_centered = B - centroid_B
    
    # Compute covariance matrix
    H = A_centered.T @ B_centered
    
    # SVD: Single-Value Decompasition
    U, S, Vt = np.linalg.svd(H)
    R = Vt.T @ U.T
    
    # Handle reflection case
    if np.linalg.det(R) < 0:
        Vt[-1, :] *= -1
        R = Vt.T @ U.T
    
    # Translation
    t = centroid_B - R @ centroid_A
    
    return R, t

points_A = np.array(unityPointsArray)
points_B = np.array(xrPointsArray)

R, t = rigid_transform_3D(points_A, points_B)
points_B_aligned = (R @ points_A.T).T + t
# print(points_B_aligned.shape)

allignedPointsArray = []
for idx in range(len(xrCoordinates)):
    allignedPointsArray.append(points_B_aligned[idx, :])
    points = np.array(allignedPointsArray)
    rr.set_time("time", timestamp=datetime.fromisoformat(xrCoordinates[idx].TimeStamp))
    rr.log("paths/allignedPath/points", rr.Points3D(points, colors=[[0, 0, 255]], radii=0.08))
# endregion

# region Beacons
beaconPoseList = []
for bcn in beacons:
    beaconPoseList.append(switchHandness([bcn.xpos, bcn.ypos, bcn.zpos]))

# transfromedBeaconPose = (R @ np.array(beaconPoseList).T).T + t

rr.log("beacons", rr.Points3D(np.array(beaconPoseList), colors=[[186, 3, 252]], radii=0.1), static=True)
# endregion
# endregion

rr.log("testpoint", rr.Points3D(np.array([0.17, 4.64, -7.35]), colors=[[186, 3, 252]], radii=0.1), static=True)
# endregion