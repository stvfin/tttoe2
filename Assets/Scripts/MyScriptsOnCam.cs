using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;

public class MyScriptsOnCam : MonoBehaviour {
  private static string filepath = "";      //Путь сохранения состояния
  public Slider slider;             //Слайдер звука
  public AudioSource audio_src;     //Аудиопроигрыватель
  private float snd_main_volume = 0.35f;    //Исходный уровень звука
  public Image panel_snd;           //Панель звука
  public Image vpanel_snd;          //Панель звука внутренняя
  public Image btn_snd;             //Кнопка звука
  public Text vol_snd;              //Уровень звука
  public Image panel_start;         //Панель старт
  public Image panel_table;         //Главная панель игры
  public Image panel_vicerr;        //Панель победы или проигрыша
  public Image vpanel_vicerr;       //Панель победы или проигрыша внутр
  public Text txt_vicerr;           //Надпись Победа-Ошибка
  public Image button_big;          //Большой квадрат
  public Image button_little;       //Малый квадрат
  public Image img_vic4, img_vic5, img_vic6;  //Для анимации
  private bool start_game = false;  //Режим запуска
  private bool victory = false;     //Режим анимации победы
  private int vic_circle_max = 100; //100/50 - 2 сек анимации
  private int vic_circle_wrk = 0;   //для анимации
  private Vector3 scale4, scale5, scale6; //сохранение положения 4,5,6 для анимации

  public void Start() {
#if UNITY_ANDROID && !UNITY_EDITOR
        filepath = Path.Combine(Application.persistentDataPath, "gsave.sav");
#else
    filepath = Path.Combine(Application.dataPath, "gsave.sav"); 
#endif
    Debug.Log("Mysave Path=" + filepath);
    LoadPrim();
    audio_src.volume = snd_main_volume;
    
    //Для отладки - устанавливает/убирает активные панели
    /*
    panel_table.gameObject.SetActive(false);  //Отключить главную
    panel_snd.gameObject.SetActive(false);    //Отключить панель звука
    panel_start.gameObject.SetActive(true);   //Включить стартовую панель
    panel_vicerr.gameObject.SetActive(false); //Отключить панель победы
    */
    float ratio = 1920f / (float)Screen.height;     //Подгонка размера панелей
    panel_start.gameObject.transform.localScale = new Vector3(4, 4, 0) / ratio;
    panel_table.gameObject.transform.localScale = new Vector3(4, 4, 0) / ratio;
    vpanel_vicerr.gameObject.transform.localScale = new Vector3(4, 4, 0) / ratio;
    vpanel_snd.gameObject.transform.localScale = new Vector3(4, 4, 0) / ratio;
    btn_snd.gameObject.transform.localScale = new Vector3(4, 4, 0) / ratio;
    btn_snd.GetComponent<RectTransform>().anchoredPosition = btn_snd.GetComponent<RectTransform>().anchoredPosition / ratio;
  }

  public void Update() {
    if (panel_snd.gameObject.activeSelf) {
      audio_src.volume = slider.value;
      vol_snd.text = "" + (int)(slider.value * 100) + "%";
      SavePrim();     //Сохранение уровня звука
    } else if (!start_game) panel_start.gameObject.SetActive(true);
  }

  public void FixedUpdate() {
    if (victory) {        //Анимация победы
      if (vic_circle_wrk++ < vic_circle_max) {
        img_vic4.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0) * (vic_circle_wrk % 20);
        img_vic5.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0) * (vic_circle_wrk % 20);
        img_vic6.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0) * (vic_circle_wrk % 20);
        img_vic4.gameObject.transform.eulerAngles = new Vector3(0, 0, 15) * (vic_circle_wrk % 20);
        img_vic5.gameObject.transform.eulerAngles = new Vector3(0, 0, 15) * (vic_circle_wrk % 20);
        img_vic6.gameObject.transform.eulerAngles = new Vector3(0, 0, 15) * (vic_circle_wrk % 20);
      } else {    //восстановление после анимации победы
        victory = false;
        img_vic5.gameObject.SetActive(false);
        panel_vicerr.gameObject.SetActive(true);
        button_big.gameObject.SetActive(true);
        button_little.gameObject.SetActive(false);
        img_vic4.gameObject.transform.localScale = scale4;
        img_vic5.gameObject.transform.localScale = scale5;
        img_vic6.gameObject.transform.localScale = scale6;
        img_vic4.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        img_vic4.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        img_vic4.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
      }
    }
  }

  public void SetSnd() {    //Кнопка Звук
    if (!victory) {
      if (!start_game) panel_start.gameObject.SetActive(false);
      panel_snd.gameObject.SetActive(!panel_snd.gameObject.activeSelf);
    }
  }

  public void BtnStart() {  //Кнопка Start
    start_game = true;
    panel_start.gameObject.SetActive(false);  //Отключить стартовую панель
    panel_table.gameObject.SetActive(true);   //Включить таблицу
  }

  public void BtnLittle() { //Кнопка балласт
    if (!victory) {
      button_big.gameObject.SetActive(false);
      button_little.gameObject.SetActive(true);
    }
  }
  public void BtnBig() {    //Кнопка Большой балласт
    if (!victory) {
      button_big.gameObject.SetActive(true);
      button_little.gameObject.SetActive(false);
    }
  }
  public void BtnsTable() {         //ОШИБКА
    if (!victory) {
      txt_vicerr.text = "ОШИБКА";
      panel_vicerr.gameObject.SetActive(true);
    }
  }
  public void BtnsTableVic() {      //ПОБЕДА
    if (!victory) {
      txt_vicerr.text = "ПОБЕДА";
      victory = true;
      vic_circle_wrk = 0;
      img_vic5.gameObject.SetActive(true);
      button_big.gameObject.SetActive(false);
      button_little.gameObject.SetActive(false);
      scale4 = img_vic4.gameObject.transform.localScale;  //Сохранение масштаба квадратиков
      scale5 = img_vic5.gameObject.transform.localScale;
      scale6 = img_vic6.gameObject.transform.localScale;
    }
  }
  public void BtnReplay() {       //Кнопка еще раз
    panel_vicerr.gameObject.SetActive(false);
    button_big.gameObject.SetActive(true);      //включить большой балласт
    button_little.gameObject.SetActive(false);  //выключить малый балласт
  }
  private void SavePrim() {                 //Сохранение данных и даты
    BinaryFormatter bf = new BinaryFormatter();
    FileStream fs = new FileStream(filepath, FileMode.Create);
    SaveData sv = new SaveData();
    string s_date = System.DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
    sv.date_save = s_date;
    sv.snd_save = slider.value;
    bf.Serialize(fs, sv);                       //Сериализация
    fs.Close();
  }
  private void LoadPrim() {               //Загрузка данных
    if (!File.Exists(filepath)) return;
    BinaryFormatter bf = new BinaryFormatter();
    FileStream fs = new FileStream(filepath, FileMode.Open);
    SaveData sv = new SaveData();
    sv = (SaveData)bf.Deserialize(fs);
    fs.Close();
    snd_main_volume = sv.snd_save;            //уровень звука
    slider.value = snd_main_volume;
  }
}
[System.Serializable]
public class SaveData {
  public float snd_save;
  public string date_save;
}