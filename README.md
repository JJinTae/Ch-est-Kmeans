# GPU를 이용한 순차적 K-means 군집화

> 본 프로젝트는 [GPU를 이용한 K-means 군집화 기반 채널추정 연산속도 개선](https://www.dbpia.co.kr/journal/articleDetail?nodeId=NODE09346734) 논문을 소개하는 [사이트](http://13.124.195.121/blog/)의 서버(AWS) 중단을 대비하여 작성하였습니다. 문제가 있을시 바로 삭제하겠습니다.
<br>
Python Project : https://github.com/JJinTae/Kmeans_Clustering_Python
<br>
<br>

순차적 K-평균 군집화(sequential K-means Clustering, SKC) 기법을 이용하여 파일럿 신호 전송 없이 데이터 신호만으로 16QAM 이상의 고차 변조 신호에 대한 채널 추정 및 성능을 확인했다.
<br><br>
그러나 군집화 알고리즘의 계산 복잡도가 높아 연산 속도가 느렸다. GPU를 이용한 순차적 K-means 군집화는 기존에 CPU를 이용해 군집화 연산을 했지만,
<br><br>
Altimesh Hybridizer를 적용하여 새로 만든 알고리즘에 계산 복잡도가 높은 군집화 연산 부분을 GPU(RTX 2080Ti)를 이용해 연산함으로써 데이터 심볼의 수가 일정 수준 이상으로 많아 졌을 때 연산시간이 기존의 CPU연산보다 GPU연산이 더 짧은 시간에 연산을 끝낼 수 있게 하였다.
<br><br>
    <img src = "https://user-images.githubusercontent.com/46085058/100539921-2e0f7d80-327d-11eb-99e1-397195ff7456.jpg" width = 40%>
    <figcaption>그림(1) QPSK 실행시간</figcaption>
</figure>
<br>
<figure>
    <img src = "https://user-images.githubusercontent.com/46085058/100540320-053cb780-3280-11eb-948d-b1f2b61c3196.jpg" width = 40%>
    <figcaption>그림(2) 16QAM 실행시간</figcaption>
</figure>
<br><br>

그림(1)과 그림(2)의 그래프를 보면 데이터 심볼의 수가 일정 수준 이상을 넘어섰을 때 실행시간의 차이가 **기존 CPU연산보다 GPU연산이 더 짧아지는 결과를 확인하였다.**
<br><br>
이로써 기존 CPU연산 보다 빠른 GPU연산을 구현하여 일정 수준 이상의 데이터 심볼 수를 군집화하는 과정에서 속도 개선을 달성하였다.
<br><br>
본 페이지에서는 **Altimesh Hybridizer**라는 프로그램을 이용해 C++에서나 사용할 수 있는 GPU연산을 C#으로도 가능하게 하여 누구나 쉽게 병렬연산을 구현할 수 있도록 설명하기 위해 제작되었다.
<br>
## Kmeans Using CUDA
군집화 과정 중 임의로 생성해준 중심점과 데이터 심볼과의 거리를 측정하는 과정이다. Ex) QPSK_Kmeans
<br>
![기존 Kmeans 연산 과정](https://user-images.githubusercontent.com/46085058/100540734-74b3a680-3282-11eb-9964-a6ac56de7636.png)<br>
이와 같은 기존 CPU연산을 이용한 Kmeans 알고리즘 연산 과정을 다음과 같이 GPU연산을 사용함으로써 실행시간의 차이가 더 짧아질 수 있도록 구현하였다.
<br>
![image](https://user-images.githubusercontent.com/46085058/100541277-138dd200-3286-11eb-9d96-b77b2214548b.png)<br>
CPU 연산방법과 GPU 연산방법의 차이
<br>
<br>
![image](https://user-images.githubusercontent.com/46085058/100541281-24d6de80-3286-11eb-84fc-1e6e4f5da2ec.png)<br>
Kmeans 군집화 연산 과정중  가장 시간이 오래걸리는 알고리즘을 GPU로 연산을 하기 위한 **병렬 연산** 코드 부분이다.
<br>
## Cuda Toolkit 설치방법
아래 링크 접속
<p><a href="https://developer.nvidia.com">https://developer.nvidia.com</a></p>

![image](https://user-images.githubusercontent.com/46085058/100541344-c2caa900-3286-11eb-9a91-dd7c59a30b24.png)
<br><br><br><br>
![image](https://user-images.githubusercontent.com/46085058/100541379-f86f9200-3286-11eb-894f-3b3e5b040158.png)
<br><br><br><br>
![image](https://user-images.githubusercontent.com/46085058/100541394-13420680-3287-11eb-84c4-d78fc0800091.png)
<br><br><br><br>
![image](https://user-images.githubusercontent.com/46085058/100541407-22c14f80-3287-11eb-99cc-50a5b24c1c50.png)
<br>
CUDA Toolkit 설치 유튜브 영상 참고 : [https://youtu.be/cL05xtTocmY](https://youtu.be/cL05xtTocmY)
<br>
## Altimesh Hybridizer 설치방법

![image](https://user-images.githubusercontent.com/46085058/100541448-5dc38300-3287-11eb-8e4f-6c6049bebc0e.png)

### Altimesh Hybridizer란?
.NET 어셈블리 (MSIL) 또는 Java 아카이브 (java bytecode)에서 벡터화 된 C ++ 소스 코드 (AVX) 및 CUDA C 소스 코드를 생성하는 고급 생산성 도구입니다.
<br><br>
개발 환경에서 개발자는  virtual functions  및  generics을 사용할 수 있지만 프로세서 및 메모리의 최대 성능을 ~ 80 % 사용하여 GPU의 컴퓨팅 기능을 효율적으로 사용할 수 있습니다. 
<br><br>
출처 : [https://developer.nvidia.com/altimesh-hybridizer](https://developer.nvidia.com/altimesh-hybridizer)

### Altimesh Hybridizer 설치 방법
먼저 CUDA Toolkit이 설치 되어있는지 확인 후 설치를 진행하세요.<br>
[CUDA Toolkit 설치하러가기](https://github.com/JJinTae/Ch-est-Kmeans/new/master?readme=1#cuda-toolkit-%EC%84%A4%EC%B9%98%EB%B0%A9%EB%B2%95)
<br><br>
Visual Studio 도구 탭 - > 확장 및 업데이트 - > 온라인 - > Altimesh Hybridizer 검색 후 설치<br>
![image](https://user-images.githubusercontent.com/46085058/100541527-da566180-3287-11eb-9564-697aab55c8a0.png)
<br><br>
설치됨 - > 템플릿 - > Visual C# - > 창  탭에서
Altimesh Hybridizer 설치한 버전과 동일한 버전의 프로젝트 생성
<br>
![image](https://user-images.githubusercontent.com/46085058/100541543-f6f29980-3287-11eb-92ee-2c0a6a8ce5a1.png)
<br><br>
프로젝트 - > 참조 추가 탭에서<br>
프로젝트 -> 솔루션 -> 이전에 생성한 프로젝트 체크
<br>
![image](https://user-images.githubusercontent.com/46085058/100541552-096cd300-3288-11eb-98c8-bce53e11c318.png)
<br><br>
이후 Hybridizer탭이 생성된 것을 확인할 수 있습니다.<br>
Hybridizer - > License Settings
<br>
![image](https://user-images.githubusercontent.com/46085058/100541576-34efbd80-3288-11eb-8cf9-c080538a1f7c.png)
<br>
<br>
이메일 입력후 Subscribe 버튼 클릭 후 입력하신 메일을 확인하여 코드를 입력합니다.
<br>
![image](https://user-images.githubusercontent.com/46085058/100541612-6e282d80-3288-11eb-84ff-4c2cf08c8c8f.png)
<br>

> ※ 라이선스는 사용 기간중 한번만 인증하면 됩니다. 라이선스가 만료되면 Subscriptionl의 시리얼번호를 지우고 이메일을 입력하면 새로운 라이센스를 받을 수 있습니다.

Altimesh Hybridizer 프로젝트 만들기 : [https://www.youtube.com/watch?v=ZbA-2XO1JP0](https://www.youtube.com/watch?v=ZbA-2XO1JP0)
