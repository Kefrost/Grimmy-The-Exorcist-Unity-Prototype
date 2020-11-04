using UnityEngine;

public class InputManager
{
    private static InputManager instance = null;

    public static InputManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputManager();
            }

            return instance;
        }
    }

    private bool isVerticalInUse;
    private bool isHorizontalInUse;

    private float VerticalAxis => Input.GetAxisRaw("Vertical") * -1f;
    private float HorizontalAxis => (int)(Input.GetAxisRaw("Horizontal"));

    public void HandleInputManagement(ref int currentIndex, int actionsCount, bool isDoubleAxis)
    {
        if (isDoubleAxis)
        {
            if (VerticalAxis > 0 && !isVerticalInUse)
            {
                if (currentIndex < actionsCount - 2)
                {
                    currentIndex += 2;
                    isVerticalInUse = true;
                }
            }
            else if (VerticalAxis < 0 && !isVerticalInUse)
            {
                if (currentIndex >= actionsCount - 2)
                {
                    currentIndex -= 2;
                    isVerticalInUse = true;
                }
            }
            else if (HorizontalAxis > 0 && !isHorizontalInUse)
            {
                if (currentIndex < actionsCount - 1)
                {
                    currentIndex += 1;
                    isHorizontalInUse = true;
                }
            }
            else if (HorizontalAxis < 0 && !isHorizontalInUse)
            {
                if (currentIndex > 0)
                {
                    currentIndex -= 1;
                    isHorizontalInUse = true;
                }
            }

            if (VerticalAxis == 0)
            {
                isVerticalInUse = false;
            }

            if (HorizontalAxis == 0)
            {
                isHorizontalInUse = false;
            }
        }
        else
        {
            int axis = (int)(Input.GetAxisRaw("Vertical") * -1f);

            if (currentIndex + axis < actionsCount && currentIndex + axis >= 0 && !isVerticalInUse)
            {
                currentIndex += axis;
                isVerticalInUse = true;
            }

            if (Input.GetAxisRaw("Vertical") == 0)
            {
                isVerticalInUse = false;
            }
        }
    }
}
