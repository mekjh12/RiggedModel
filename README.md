# Animate 클래스 사용법
### XmlDae 
  - RawModel3d 가져오기
```c#
xmlDae = new XmlDae(EngineLoop.PROJECT_PATH + "\\Res\\guybrush.dae");
```
  - Action 가져오기 (Action 파일의 생성법은 아래 참고)
```c#
xmlDae.AddAction(EngineLoop.PROJECT_PATH + "\\Res\\Action\\Jump.dae")
```


### Entity

### AniModel
```mermaid
graph TD;
    XmlDae-->AniModel;
    XmlDae-->Entity;
    Entity-->AniModel;
```

# Mixamo 사용법
### 캐릭터 가져오기
* 블렌더 -> Export (Fbx) -> PathMode(Copy) -> Embed Textures
 ![image](https://github.com/mekjh12/RiggedModel/assets/122244587/e888029e-030c-47fd-b388-608b4438bed3)

* Mixamo -> Upload Character -> 동그라미로 Rig 설정

### Weight 적용하기
* 자동으로 Bone에 따른 Weight를 적용함.
   
### 캐릭터 내보내기
 
![image](https://github.com/mekjh12/RiggedModel/assets/122244587/a00a34f2-5987-4975-a973-3886e3f2211a)

### Action 내보내기
