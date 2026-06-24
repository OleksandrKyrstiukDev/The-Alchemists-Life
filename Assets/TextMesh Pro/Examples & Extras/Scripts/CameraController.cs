using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraModes { Follow, Isometric, Free }


        private Transform cameraTransform;
        private Transform dummyTarget;


        public Transform CameraTarget;


        public float FollowDistance = 30.0f;
        public float MaxFollowDistance = 100.0f;
        public float MinFollowDistance = 2.0f;


        public float ElevationAngle = 30.0f;
        public float MaxElevationAngle = 85.0f;
        public float MinElevationAngle = 0f;


        public float OrbitalAngle = 0f;


        public CameraModes CameraMode = CameraModes.Follow;


        public bool MovementSmoothing = true;
        public bool RotationSmoothing = false;

        private bool previousSmoothing;


        public float MovementSmoothingValue = 25f;
        public float RotationSmoothingValue = 5.0f;



        [Header("Sensitivity")]
        [SerializeField]
        private float defaultSensitivity = 2f;


        private float MoveSensitivity;



        private Vector3 currentVelocity = Vector3.zero;

        private Vector3 desiredPosition;

        private float mouseX;

        private float mouseY;

        private Vector3 moveVector;

        private float mouseWheel;



        void Awake()
        {
            if (QualitySettings.vSyncCount > 0)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = -1;



            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.Android)
            {
                Input.simulateMouseWithTouches = false;
            }



            cameraTransform = transform;

            previousSmoothing = MovementSmoothing;
        }





        void Start()
        {

            // Завантаження чутливості з налаштувань
            MoveSensitivity =
                PlayerPrefs.GetFloat(
                    "Sensitivity",
                    defaultSensitivity
                );



            if (CameraTarget == null)
            {
                dummyTarget =
                    new GameObject(
                        "Camera Target"
                    ).transform;


                CameraTarget = dummyTarget;
            }
        }







        void LateUpdate()
        {
            GetPlayerInput();



            if (CameraTarget != null)
            {


                if (CameraMode == CameraModes.Isometric)
                {

                    desiredPosition =
                        CameraTarget.position +
                        Quaternion.Euler(
                            ElevationAngle,
                            OrbitalAngle,
                            0f
                        ) *
                        new Vector3(
                            0,
                            0,
                            -FollowDistance
                        );
                }



                else if (CameraMode == CameraModes.Follow)
                {

                    desiredPosition =
                        CameraTarget.position +
                        CameraTarget.TransformDirection(
                            Quaternion.Euler(
                                ElevationAngle,
                                OrbitalAngle,
                                0f
                            ) *
                            new Vector3(
                                0,
                                0,
                                -FollowDistance
                            )
                        );
                }



                if (MovementSmoothing)
                {

                    cameraTransform.position =
                        Vector3.SmoothDamp(
                            cameraTransform.position,
                            desiredPosition,
                            ref currentVelocity,
                            MovementSmoothingValue *
                            Time.fixedDeltaTime
                        );

                }

                else
                {
                    cameraTransform.position =
                        desiredPosition;
                }




                if (RotationSmoothing)
                {

                    cameraTransform.rotation =
                        Quaternion.Lerp(
                            cameraTransform.rotation,
                            Quaternion.LookRotation(
                                CameraTarget.position -
                                cameraTransform.position
                            ),
                            RotationSmoothingValue *
                            Time.deltaTime
                        );
                }

                else
                {
                    cameraTransform.LookAt(CameraTarget);
                }

            }
        }








        void GetPlayerInput()
        {

            moveVector = Vector3.zero;



            mouseWheel =
                Input.GetAxis(
                    "Mouse ScrollWheel"
                );



            if (Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift))
            {



                mouseWheel *= 10;



                if (Input.GetMouseButton(1))
                {

                    mouseY =
                        Input.GetAxis(
                            "Mouse Y"
                        );


                    mouseX =
                        Input.GetAxis(
                            "Mouse X"
                        );




                    if (Mathf.Abs(mouseY) > 0.01f)
                    {

                        ElevationAngle -=
                            mouseY *
                            MoveSensitivity;


                        ElevationAngle =
                            Mathf.Clamp(
                                ElevationAngle,
                                MinElevationAngle,
                                MaxElevationAngle
                            );
                    }




                    if (Mathf.Abs(mouseX) > 0.01f)
                    {

                        OrbitalAngle +=
                            mouseX *
                            MoveSensitivity;


                        if (OrbitalAngle > 360)
                            OrbitalAngle -= 360;


                        if (OrbitalAngle < 0)
                            OrbitalAngle += 360;
                    }
                }
            }




            if (mouseWheel < -0.01f ||
                mouseWheel > 0.01f)
            {

                FollowDistance -=
                    mouseWheel *
                    5f;


                FollowDistance =
                    Mathf.Clamp(
                        FollowDistance,
                        MinFollowDistance,
                        MaxFollowDistance
                    );
            }

        }
    }
}