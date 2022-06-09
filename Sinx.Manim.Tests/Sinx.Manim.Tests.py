from manimlib import *


# Manim Community

class Run(Scene):
    def construct(self):
        # 坐标系
        plane = NumberPlane()
        self.add(plane)
        v1 = Vector([1, 1])
        v2 = Vector([2, 2])
        # v1, v2 的角度
        angle = v1.get_angle(v2)
        # manimlib.utils.space_ops.get_norm(vect) 获取向量v1的模
        normalize([1, 1])
        
        self.play(ShowCreation(v1))
        # show v
        self.play(ShowCreation(v))
        
        