import subprocess
import time
from threading import *
import pyautogui


def thread_foo():
    while True:
        pyautogui.keyDown('space')
        pyautogui.keyUp('space')
        time.sleep(0.265)


screenWidth, screenHeight = pyautogui.size()
PATH_ = "C:/Users/User/Desktop/Debug/Flappy Bird with AI.exe"
thread = Thread(target=thread_foo(), args=())

game = subprocess.Popen(PATH_, shell=True, stdout=subprocess.PIPE)

time.sleep(0.78)

thread.start()