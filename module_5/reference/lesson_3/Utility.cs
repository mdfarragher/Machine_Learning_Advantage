using DlibDotNet;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ml_csharp_lesson3
{
    /// <summary>
    /// Utility class with helper methods. 
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// The Euler structure stores the three euler rotation angles.
        /// </summary>
        private struct Euler
        {
            public double yaw;
            public double pitch;
            public double roll;
        };

        /// <summary>
        /// Returns a simple face model in world coordinates. 
        /// </summary>
        /// <returns>A face model consisting of 6 landmark points in world coordinates.</returns>
        public static MatOfPoint3d GetFaceModel()
        {
            return new MatOfPoint3d(1, 6,
                new Point3d[] {
                    new Point3d(0.0f, 0.0f, 0.0f),
                    new Point3d(0.0f, -330.0f, -65.0f),
                    new Point3d(-225.0f, 170.0f, -135.0f),
                    new Point3d(225.0f, 170.0f, -135.0f),
                    new Point3d(-150.0f, -150.0f, -125.0f),
                    new Point3d(150.0f, -150.0f, -125.0f)
                });
        }

        /// <summary>
        /// Build a camera matrix with generic parameters. We assume the focal length is equal to 
        /// the image width, and the optical center is the same as the center of the image.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static MatOfDouble GetCameraMatrix(int width, int height)
        {
            return new MatOfDouble(3, 3,
                new double[] {
                    width, 0,     width / 2,
                    0,     width, height / 2,
                    0,     0,     1
                });
        }

        /// <summary>
        /// Find the Euler matrix from the output of SolvePnP.
        /// </summary>
        /// <param name="rotation">The rotation matrix returned by SolvePnp.</param>
        /// <returns>The Euler matrix containing pitch, roll, and yaw angles.</returns>
        public static MatOfDouble GetEulerMatrix(Mat rotation)
        {
            // convert the 1x3 rotation vector to a full 3x3 matrix
            var r = new MatOfDouble(3, 3);
            Cv2.Rodrigues(rotation, r);

            // set up some shortcuts to rotation matrix
            double m00 = r.At<double>(0, 0);
            double m01 = r.At<double>(0, 1);
            double m02 = r.At<double>(0, 2);
            double m10 = r.At<double>(1, 0);
            double m11 = r.At<double>(1, 1);
            double m12 = r.At<double>(1, 2);
            double m20 = r.At<double>(2, 0);
            double m21 = r.At<double>(2, 1);
            double m22 = r.At<double>(2, 2);

            // set up output variables
            Euler euler_out = new Euler();
            Euler euler_out2 = new Euler();

            if (Math.Abs(m20) >= 1)
            {
                euler_out.yaw = 0;
                euler_out2.yaw = 0;

                // From difference of angles formula
                if (m20 < 0)  //gimbal locked down
                {
                    double delta = Math.Atan2(m01, m02);
                    euler_out.pitch = Math.PI / 2f;
                    euler_out2.pitch = Math.PI / 2f;
                    euler_out.roll = delta;
                    euler_out2.roll = delta;
                }
                else // gimbal locked up
                {
                    double delta = Math.Atan2(-m01, -m02);
                    euler_out.pitch = -Math.PI / 2f;
                    euler_out2.pitch = -Math.PI / 2f;
                    euler_out.roll = delta;
                    euler_out2.roll = delta;
                }
            }
            else
            {
                euler_out.pitch = -Math.Asin(m20);
                euler_out2.pitch = Math.PI - euler_out.pitch;

                euler_out.roll = Math.Atan2(m21 / Math.Cos(euler_out.pitch), m22 / Math.Cos(euler_out.pitch));
                euler_out2.roll = Math.Atan2(m21 / Math.Cos(euler_out2.pitch), m22 / Math.Cos(euler_out2.pitch));

                euler_out.yaw = Math.Atan2(m10 / Math.Cos(euler_out.pitch), m00 / Math.Cos(euler_out.pitch));
                euler_out2.yaw = Math.Atan2(m10 / Math.Cos(euler_out2.pitch), m00 / Math.Cos(euler_out2.pitch));
            }

            // return result
            return new MatOfDouble(1, 3, new double[] { euler_out.yaw, euler_out.roll, euler_out.pitch });
            // return new MatOfDouble(1, 3, new double[] { euler_out2.yaw, euler_out2.roll, euler_out2.pitch });
        }

        /// <summary>
        /// Detect the orientation of the head in the current video frame.
        /// </summary>
        /// <param name="shape">The landmark points to use</param>
        /// <param name="cameraMatrix">The camera calibration matrix to use</param>
        /// <param name="rotationMatrix">The detected head rotation matrix</param>
        /// <param name="translationMatrix">The detected head translation matrix</param>
        /// <param name="coefficientMatrix">The detected coefficient matrix</param>
        public static void DetectHeadAngle(
            FullObjectDetection shape,
            MatOfDouble cameraMatrix,
            out Mat rotationMatrix,
            out Mat translationMatrix,
            out MatOfDouble coefficientMatrix)
        {
            // build the 3d face model
            var model = Utility.GetFaceModel();

            // build the landmark point list
            var landmarks = new MatOfPoint2d(1, 6,
                (from i in new int[] { 30, 8, 36, 45, 48, 54 }
                 let p = shape.GetPart((uint)i)
                 select new OpenCvSharp.Point2d(p.X, p.Y)).ToArray());

            // build the coefficient matrix
            var coeffs = new MatOfDouble(4, 1);
            coeffs.SetTo(0);

            // find head rotation and translation
            Mat rotation = new MatOfDouble();
            Mat translation = new MatOfDouble();
            Cv2.SolvePnP(model, landmarks, cameraMatrix, coeffs, rotation, translation);

            // return results
            rotationMatrix = rotation;
            translationMatrix = translation;
            coefficientMatrix = coeffs;
        }

        /// <summary>
        /// Draw a straight line indicating head pose on the specified image.
        /// </summary>
        /// <param name="rotation">The rotation matrix of the head.</param>
        /// <param name="translation">The translation matrix of the head.</param>
        /// <param name="cameraMatrix">The camera calibration matrix.</param>
        /// <param name="coefficients">The coefficient matrix.</param>
        /// <param name="shape">The list of facial landmark points.</param>
        /// <param name="image">The image to draw on.</param>
        public static void DrawPoseLine(
            Mat rotation,
            Mat translation,
            MatOfDouble cameraMatrix,
            MatOfDouble coefficients,
            FullObjectDetection shape,
            Bitmap image)
        {
            // create a new model point in front of the nose and project it into 2d
            var poseModel = new MatOfPoint3d(1, 1, new Point3d(0, 0, 1000));
            var poseProjection = new MatOfPoint2d();
            Cv2.ProjectPoints(poseModel, rotation, translation, cameraMatrix, coefficients, poseProjection);

            // get landmark point 30 (tip of the nose)
            var point = shape.GetPart(30);
            var tipOfNose = new OpenCvSharp.Point2d(point.X, point.Y);

            // draw a line from the tip of the nose pointing in the direction of head pose
            var p = poseProjection.At<Point2d>(0);
            var pen = new Pen(Brushes.White, 2);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.DrawLine(pen, (int)tipOfNose.X, (int)tipOfNose.Y, (int)p.X, (int)p.Y);
            }
        }

        /// <summary>
        /// Draw the landmark points on the specified image.
        /// </summary>
        /// <param name="shape">The facial landmark points.</param>
        /// <param name="image">The image to draw on.</param>
        public static void DrawLandmarkPoints(
            FullObjectDetection shape,
            Bitmap image)
        {
            using (Graphics g = Graphics.FromImage(image))
            {
                for (var i = 0; i < shape.Parts; i++)
                {
                    var point = shape.GetPart((uint)i);
                    g.FillRectangle(Brushes.White, point.X - 2, point.Y - 2, 4, 4);
                }
            }
        }

        /// <summary>
        /// Draw the eye area in the specified image.
        /// </summary>
        /// <param name="shape">The facial landmark points.</param>
        /// <param name="image">The image to draw on.</param>
        public static void DrawEyeArea(
            FullObjectDetection shape,
            Bitmap image)
        {
            // get all landmark points of the left eye
            var leftEye = (from i in Enumerable.Range(36, 6)
                           let p = shape.GetPart((uint)i)
                           select new PointF(p.X, p.Y)).ToArray();

            // get all landmark points of the right eye
            var rightEye = (from i in Enumerable.Range(42, 6)
                            let p = shape.GetPart((uint)i)
                            select new PointF(p.X, p.Y)).ToArray();

            // create a white mask of the eye areas
            using (Graphics g = Graphics.FromImage(image))
            {
                var brush = Brushes.White;
                g.FillClosedCurve(brush, leftEye);
                g.FillClosedCurve(brush, rightEye);
            }
        }
    }
}
