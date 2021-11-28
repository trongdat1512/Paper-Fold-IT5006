using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelCreator : MonoBehaviour
{
    [SerializeField] GameObject img;
    [SerializeField] Color color = Color.white;
    [SerializeField] String levelToSave = "";

    List<Part> _partList = new List<Part>();
    GameObject _centerPart;
    SpriteRenderer _centerPartRend;
    GameController _gameController;
    List<SpriteRenderer> _rendList = new List<SpriteRenderer>();

    String _savedFolderPath;
    // Start is called before the first frame update
    void Start()
    {
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
        _partList = _gameController.GetPartList();
        _centerPart = _gameController.GetCenterPart();
        _centerPartRend = _centerPart.GetComponent<SpriteRenderer>();
        foreach (var part in _partList)
        {
            _rendList.Add(part.GetComponent<SpriteRenderer>());
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        _savedFolderPath = "Assets/Resources/Sprites/Level/Level " + levelToSave + "/";
        if (Input.GetKeyDown("space"))
        {
            StampImg();
        }
        if(Input.GetKeyDown("backspace"))
        {
            LoadSavedAsset(_savedFolderPath);
            Debug.Log("Loaded");
        }
        
    }

    void StampImg()
    {
        SpriteRenderer imgRend = img.GetComponent<SpriteRenderer>();
        Sprite imgSprite = imgRend.sprite;
        Texture2D imgTexture = imgSprite.texture;
        List<Texture2D> spriteTextureList = new List<Texture2D>();
        Texture2D centerTexture = TextureFromSprite(_centerPartRend.sprite);
        List<Texture2D> textureList = new List<Texture2D>();
        List<bool> hasImg = new List<bool>();

        // Create empty texture list with color
        foreach (var part in _partList)
        {
            Texture2D tmp = TextureFromSprite(part.GetSprite());
            for (int x = 0; x < tmp.width; x++)
            {
                for (int y = 0; y < tmp.height; y++)
                {
                    if (tmp.GetPixel(x, y).a > 0)
                    {
                        tmp.SetPixel(x, y, color);
                    }
                }
            }
            spriteTextureList.Add(tmp);
            textureList.Add(tmp);
            hasImg.Add(false);
        }
        
        // Loop for each pixel in img
        for (int x = 0; x < imgTexture.width; x++)
        {
            for (int y = 0; y < imgTexture.height; y++)
            {
                Vector3 pixelWorldPos = GetPixelWorldPos(x, y, imgRend);
                Color imgPixelColor = imgTexture.GetPixel(x, y);
                
                // Find top part in pixelWorldPos
                SpriteRenderer topRend = _rendList[0];
                int maxSortingOrder = 0;
                int topRendIndex = 0;
                bool hasPart = false;
                for (int i = 0; i < _rendList.Count; i++)
                {
                    if(HasObject(pixelWorldPos, _rendList[i]))
                    {
                        hasPart = true;
                        
                        Vector2 pixelIndex = FindPixelIndex(pixelWorldPos, _rendList[i], spriteTextureList[i]);
                        int xx = (int)pixelIndex.x;
                        int yy = (int)pixelIndex.y;

                        if (_rendList[i].sortingOrder >= maxSortingOrder &&
                            spriteTextureList[i].GetPixel(xx, yy).a > 0)
                        {
                            maxSortingOrder = _rendList[i].sortingOrder;
                            topRend = _rendList[i];
                            topRendIndex = i;
                        }
                    }
                }

                if (hasPart)
                {
                    hasImg[topRendIndex] = true;
                    Vector2 topRendPixelIndex = FindPixelIndex(pixelWorldPos, topRend, spriteTextureList[topRendIndex]);
                    int xx = (int)topRendPixelIndex.x;
                    int yy = (int)topRendPixelIndex.y;
                    
                    Color topRendPixelColor = spriteTextureList[topRendIndex].GetPixel(xx, yy);

                    if (xx >= 0 && xx < spriteTextureList[topRendIndex].width &&
                        yy >= 0 && yy <= spriteTextureList[topRendIndex].height)
                    {
                        if (imgPixelColor.a > 0 && topRendPixelColor.a > 0)
                        {
                            textureList[topRendIndex].SetPixel(xx, yy, imgPixelColor);
                        }
                        else
                        {
                            textureList[topRendIndex].SetPixel(xx, yy, topRendPixelColor);
                        }
                    }
                }
                
                // Stamp white shadow to center part
                /*if(HasObject(pixelWorldPos, _centerPartRend))
                {
                    Vector2 centerPartPixelIndex = FindPixelIndex(pixelWorldPos, _centerPartRend, centerTexture);
                    int xx = (int)centerPartPixelIndex.x;
                    int yy = (int)centerPartPixelIndex.y;
                    Color centerPartPixelColor = centerTexture.GetPixel(xx, yy);
                    if (xx >= 0 && xx < centerTexture.width && yy >= 0 && yy <= centerTexture.height)
                    {
                        if (imgPixelColor.a > 0 && centerPartPixelColor.a > 0)
                        {
                            centerTexture.SetPixel(xx, yy, Color.white);
                        }
                        else
                        {
                            centerTexture.SetPixel(xx, yy, centerPartPixelColor);
                        }
                    }
                }*/
            }
        }
        
        for (int i = 0; i < _rendList.Count; i++)
        {
            textureList[i].Apply();
            Sprite stampedSprite = Sprite.Create(textureList[i],
                new Rect(0, 0, textureList[i].width, textureList[i].height),
                new Vector2(.5f, .5f));
            stampedSprite.name = "lv" + levelToSave + "_" + i;
            byte[] bytes = textureList[i].EncodeToPNG();
            if(!Directory.Exists(_savedFolderPath)) {
                Directory.CreateDirectory(_savedFolderPath);
            }
            File.WriteAllBytes(_savedFolderPath + "lv" + levelToSave + "_"+i+".png", bytes);
            _rendList[i].sprite = stampedSprite;
        }
        
        /*centerTexture.Apply();
        Sprite stampedCenterSprite = Sprite.Create(centerTexture,
            new Rect(0, 0, centerTexture.width, centerTexture.height),
            new Vector2(.5f, .5f));
        stampedCenterSprite.name = "lv" + levelToSave + "_center";
        byte[] bytess = centerTexture.EncodeToPNG();
        if(!Directory.Exists(_savedFolderPath)) {
            Directory.CreateDirectory(_savedFolderPath);
        }
        File.WriteAllBytes(_savedFolderPath + "lv" + levelToSave + "_center"+".png", bytess);
        _centerPartRend.sprite = stampedCenterSprite;*/

        Debug.Log("Saved");
    }

    void LoadSavedAsset(String savedFolderPath)
    {
        for (int i = 0; i < _rendList.Count; i++)
        {
            _partList[i].SetSprite(AssetDatabase.LoadAssetAtPath<Sprite>(savedFolderPath + "lv" + levelToSave + "_"+i+".png"));
        }

        _centerPartRend.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(savedFolderPath + "lv" + levelToSave + "_center"+".png");
    }
    static Texture2D TextureFromSprite(Sprite sprite)
    {
        if(sprite.rect.width != sprite.texture.width){
            Texture2D newText = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height);
            var newColors = sprite.texture.GetPixels((int) sprite.rect.x,
                (int) sprite.rect.y, 
                (int) sprite.rect.width, 
                (int) sprite.rect.height );
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        } else
            return sprite.texture;
    }
    
    // Get world position of a pixel in a sprite
    Vector3 GetPixelWorldPos(int xIndex, int yIndex, SpriteRenderer rend)
    {
        Bounds bounds = rend.bounds;
        Texture2D texture = TextureFromSprite(rend.sprite);
        Vector3 minPos = bounds.min;
        float boundSizeX = bounds.size.x;
        float boundSizeY = bounds.size.y;
        int width = texture.width;
        int height = texture.height;
        
        return new Vector3(
            minPos.x + xIndex * (boundSizeX/width),
            minPos.y + yIndex * (boundSizeY/height),
            minPos.z
        );
    }

    bool HasObject(Vector3 pos, SpriteRenderer rend)
    {
        Bounds bounds = rend.bounds;
        Vector3 minPos = bounds.min;
        Vector3 maxPos = bounds.max;

        if (pos.x >= minPos.x && pos.x <= maxPos.x &&
            pos.y >= minPos.y && pos.y <= maxPos.y)
        {
            return true;
        }
        return false;
    }
    
    // Find pixel index of a sprite in a certain world position
    Vector2 FindPixelIndex(Vector3 pos, SpriteRenderer rend, Texture2D texture)
    {
        int xIndex, yIndex;
        Bounds bounds = rend.bounds;
        Vector3 minPos = bounds.min;
        float boundSizeX = bounds.size.x;
        float boundSizeY = bounds.size.y;
        int width = texture.width;
        int height = texture.height;
        
        xIndex = (int) Math.Floor((pos.x - minPos.x) / (boundSizeX / width));
        yIndex = (int) Math.Floor((pos.y - minPos.y) / (boundSizeY / height));
        if (xIndex >= width) xIndex = width - 1;
        if (yIndex >= height) yIndex = height - 1;

        if (Math.Abs((int) Math.Floor(rend.transform.rotation.x)) == 1)
        {
            yIndex = height - 1 - yIndex;
        }
        if (Math.Abs((int) Math.Floor(rend.transform.rotation.y)) == 1)
        {
            xIndex = width - 1 - xIndex;
        }
        if ((int) Math.Floor( Math.Abs(rend.transform.rotation.x * 10)) == 7 &&
            (int) Math.Floor( Math.Abs(rend.transform.rotation.y * 10)) == 7)
        {
            int tmp = xIndex;
            xIndex = height - 1 - yIndex;
            yIndex = width - 1 - tmp;
        }
        
        return new Vector2(xIndex, yIndex);
    }
}
