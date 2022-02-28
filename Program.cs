using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace packbits
{
    internal class Program

    {
        static void Main(string[] args)
        {
            var lines = Console.ReadLine().Split(' '); //半角空白区切りで分割して配列で取得
            var result = new List<string>(); //結果格納リスト

            //圧縮 
            if (lines[0] == "compress") result = Compress(lines);
            //解凍
            if (lines[0] == "decompress") result = decompress(lines);
            
            result.ForEach(i => Console.Write(i)); //結果表示
            Console.ReadLine(); //終了
        }


        /// <summary>
        /// 圧縮
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        static List<string> Compress(string[] lines)
        {
            var result = new List<string>(); //結果格納リスト
            var comStr = lines[1].ToCharArray(); //圧縮対象となる文字列を1文字ずつ格納(charで取得しstringへ変換)
            var matchQ = new Queue<string>(); //一致のキュー
            var notMatchQ = new Queue<string>(); //不一致のキュー
            var firstLoop = true;　//１回目のループ
            string preTmp = null; //１つ前の文字

            //圧縮対象文字列数だけループ
            foreach (var c in comStr)
            {
                //ループ１回目
                if (firstLoop) firstLoop = false; //１回目で切る

                //1つ前の文字と一致しているのかをチェック
                else if (preTmp == c.ToString())
                {
                    //不一致キューに格納がある場合
                    if (notMatchQ.Count > 0)
                    {
                        result.Add("-" + notMatchQ.Count); //頭に「-」+「格納数」を格納
                        foreach (var i in notMatchQ) result.Add(i); //不一致キューに格納されている文字を１文字ずつ格納
                        notMatchQ.Clear(); //キューのリセット
                    }
                    matchQ.Enqueue(preTmp);  //一致キューに格納
                }

                //不一致
                else
                {
                    //一致キューに格納がある場合
                    if (matchQ.Count() > 0)
                    {
                        matchQ.Enqueue(preTmp);  //preTmp文字を格納(一致しているため)
                        result.Add(matchQ.Count().ToString() + matchQ.Dequeue()); //「一致文字数」+「一致キューの先頭文字」
                        matchQ.Clear(); //キューのリセット
                    }
                    else notMatchQ.Enqueue(preTmp); //不一致キューに格納
                }
                preTmp = c.ToString(); //１つ前の文字として保持
            }

            /*ループを抜け、最後の処理*/
            //一致キューに格納がある場合
            if (matchQ.Count() > 0)
            {
                matchQ.Enqueue(preTmp);  //preTmp文字を格納(一致しているため)
                result.Add(matchQ.Count().ToString() + matchQ.Dequeue()); //「一致文字数」+「一致キューの先頭文字」
            }
            //不一致キューに格納がある場合
            else
            {
                //
                notMatchQ.Enqueue(preTmp);
                result.Add("-" + notMatchQ.Count); //頭に「-」+「格納数」を格納
                foreach (var i in notMatchQ) result.Add(i); //不一致キューに格納されている文字を１文字ずつ格納
            }
            return result;
        }


        /// <summary>
        /// 解凍
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        static List<string> decompress(string[] lines)
        {
            var result = new List<string>(); //結果格納リスト
            var decStr = lines[1].ToCharArray();
            var countQ = new Queue<string>(); //キュー
            var isMinus = false; //先頭がマイナス(-)かを管理するフラグ
            string countStr = null;
            int? countInt = null;
            string preTmp = null;

            foreach (var d in decStr)
            {
                //-
                if (d.ToString() == "-") isMinus = true;
                //数値
                else if (Regex.IsMatch(d.ToString(), "[0-9]"))
                {
                    //1つ前の文字が"-"でない
                    if (preTmp != "-") isMinus = false;
                    countQ.Enqueue(d.ToString());
                }

                //文字
                else
                {
                    if (isMinus)
                    {
                        //リストのリセット
                        countInt = 0;
                        countStr = null;
                        countQ.Clear();//キューのリセット
                        result.Add(d.ToString());
                    }
                    else
                    {
                        //１つの数字とする
                        foreach (var j in countQ) countStr = countStr + j.ToString(); //文字で桁ごとに足す       
                        countInt = int.Parse(countStr); //数値に変換
                        //カウント数分、対象文字をリストに格納
                        for (int k = 0; countInt > k; k++) result.Add(d.ToString());
                        countQ.Clear(); //キューのリセット
                        //リストのリセット
                        countInt = 0;
                        countStr = null;
                    }
                }
                preTmp = d.ToString(); //１つ前を保持
            }
            return result;
        }
    }
}
