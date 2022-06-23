using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Paint : MonoBehaviour
{
    [SerializeField] private double paintedPixels;
    [SerializeField] private double totalPixels;
    [SerializeField] private int penSize = 5;
    [SerializeField] private Vector2 touchPos;
    [SerializeField] private Vector2 lastTouchPos;
    public double paintedPercentage;
    public Camera cam;
    public Board board;
    public Text percentageText;
    private bool touchedLastFrame;
    private int[,] pixels;
    private RaycastHit hit;
    private string percentage;

    // Start is called before the first frame update
    void Start()
    {
        pixels = new int[(int)(board.textureSize.x), (int)(board.textureSize.y)];
        totalPixels = (int)(board.textureSize.x) * (int)(board.textureSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        paintedPercentage = paintedPixels / totalPixels * 100;
        percentageText.text = "Almost done:"+ "%" + string.Format("{0:0.00}", paintedPercentage);
        if (Input.GetMouseButtonDown(0))
        {
            percentageText.gameObject.SetActive(true);
            var Ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Ray, out hit))
            {
                if (hit.transform.CompareTag("Board"))
                {

                    touchPos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);

                    var x = (int)(touchPos.x * board.textureSize.x - (penSize / 2));
                    var y = (int)(touchPos.y * board.textureSize.y - (penSize / 2));
                    lastTouchPos = new Vector2(x, y);
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Draw();
        }

        if (paintedPercentage>=98.5f) // if left a little area without painted, complete %100
        {
            paintedPercentage = 100;
            percentageText.text = "Done "+" %100";
        }

    }

    private void Draw()
    {
        var Ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(Ray, out hit))
        {
            if (hit.transform.CompareTag("Board"))
            {
                if (board == null)
                {
                    board = hit.transform.GetComponent<Board>();
                }
                touchPos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
                var x = (int)(touchPos.x * board.textureSize.x - (penSize / 2));
                var y = (int)(touchPos.y * board.textureSize.y - (penSize / 2));
                if (touchedLastFrame)
                {
                    for (float f = 0.01f; f < 1.00f; f += 0.01f)
                    {
                        var lerpX = (int)Mathf.Lerp(lastTouchPos.x, x, f);
                        var lerpY = (int)Mathf.Lerp(lastTouchPos.y, y, f);
                        if (Input.GetMouseButtonUp(0))
                        {
                            return;

                        }
                        for (int i = -penSize; i < penSize / 2; i++)
                        {
                            for (int j = -penSize; j < penSize / 2; j++)
                            {
                                if (lerpX + i > 0 && lerpX + i < board.textureSize.x && lerpY + j > 0 && lerpY + j < board.textureSize.y)
                                {
                                    if (pixels[lerpX + i, lerpY + j] != 1)
                                    {
                                        pixels[lerpX + i, lerpY + j] = 1;
                                        paintedPixels++;
                                        board.texture.SetPixel(lerpX + i, lerpY + j, Color.red);
                                    }
                                }
                            }
                        }
                    }
                    board.texture.Apply();
                }
                lastTouchPos = new Vector2(x, y);
                touchedLastFrame = true;
                return;
            }
            else
            {
                touchedLastFrame = false;
                return;
            }
        }

        //wall = null;
        touchedLastFrame = false;
    }
}
