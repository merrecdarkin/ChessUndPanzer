using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    protected Transform playerTransform;// Player Transform
    protected GameObject playerTanker;// Player Transform

	public GameObject centralPoint;
    public Vector3 forwardpos;
    public Vector3 Triangupos;  
    public bool IsActive; public int UnitValue;
   /* public float fieldboundMaxX=27.5f;
    public float fieldboundMinX=-7.5f;
    public float fieldboundMaxZ=27.5f;
    public float fieldboundMinZ=-7.5f;*/

    public int relativeX;   
    public int relativeZ;
    public float PosibleDis1;     public float PosibleDis3;
    public float PosibleDis2;     public float PosibleDis4;
    public float PosibleDis5;     public float PosibleDis6;
    public float PosibleDis7;     public float PosibleDis8;



    protected bool CaptureAt1;     protected bool CaptureAt2;
    protected bool CaptureAt3;     protected bool CaptureAt4;
    protected bool[] CaptureLocate; public bool FriendRally;
    
    public List<GameObject> CaptureTargets = new List<GameObject>();

	protected AudioSource MoveingSound; public AudioSource tremmorSound;


	public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        Lord,
        Devil
    }
	public PieceType TypeChess;    public  bool killMode=true;
	public float elapsedTime; public float stunnedTimer;
	protected int xax;
	protected int zax;  protected int KnightPhase=-1;     public int patrolPawnState; protected Vector3 PatrolArea; public bool Rallying;
    public float promotetionZ;          
     public GameObject NewShell;     //Child components


    private Rigidbody _rigidbody; // The rigidbody of the chaser

    protected GameObject[] WhiteSide;    
    protected GameObject[] RedSide;    
    public GameObject[] CurrentSide;     public GameObject[] OtherSide; 
    public GameObject Spawnder; public GameObject SpawnderLocation; public GameObject[] PlatesEnemmy;  



    protected int Armynumber; public int HP; protected int HijackSelect;

    // Start is called before the first frame update
    void Start(){
        Rallying=false;
        float relavXFloat =(12.5f+transform.position.x)/5.0f; 
        relativeX= (int)relavXFloat;
        IsActive=true;
        float relavZFloat =(12.5f+transform.position.z)/5.0f; 
        relativeZ= (int)relavZFloat;

        FriendRally= false; PatrolArea=transform.position; patrolPawnState=0;

        _rigidbody=GetComponent<Rigidbody>();   MoveingSound= GetComponent<AudioSource>(); 
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        GameObject PromoteDist = GameObject.FindGameObjectWithTag("PromoteFlag");
        promotetionZ= PromoteDist.transform.position.z;
        playerTransform = objPlayer.transform;
        playerTanker = objPlayer;

        centralPoint = GameObject.FindGameObjectWithTag("CentralPoint");

        RedSide=  GameObject.FindGameObjectsWithTag("Enemy");
        WhiteSide=  GameObject.FindGameObjectsWithTag("Friendly");

        elapsedTime = 0.0f; 
        Armynumber=-1;
        CurrentSide=WhiteSide; OtherSide=RedSide;
        if (gameObject.tag=="Enemy"){CurrentSide=RedSide; OtherSide=WhiteSide;} //Side correction for future reference;
        for (int i = 0; i < CurrentSide.Length; i++) { //Ensure their positon is correctly assingned as their unit position
            if (CurrentSide[i].transform.position==transform.position){
                Armynumber=i;
            }
        }
        
        xax=0; zax=0;
        
        forwardpos=transform.position;
        if(!playerTransform)
                print("Player doesn't exist.. Please add one with Tag named 'Player'");
    }

    // Update is called once per frame
    void Update()
    {
        // Select behavior
        if (IsActive){
            if (elapsedTime>5.0f){killMode=true;}//gt out of stunt state
            switch (TypeChess) {
                case PieceType.Pawn: UpdateMovePawn(); break;
                case PieceType.Bishop: UpdateMoveBishop(); break;
                case PieceType.Knight: UpdateMoveKnight(); break;
                case PieceType.Rook: UpdateMoveRook(); break;
                case PieceType.Queen: UpdateMoveQueen(); break;

            }
        }
        
    // Update the time
        elapsedTime += Time.deltaTime;
        if (gameObject.tag=="Enemy"){elapsedTime +=0.005f;} //Enemy Side is stronger;
    }

    //Pawn Movement: 
    //      X <-If ally
    //     Pawn
    //      X <-If enemy
    protected void UpdateMovePawn(){
            if ((elapsedTime >= 5.5) && (transform.position==forwardpos)) {
                elapsedTime = 0.0f;
                killMode=true;         ToggleAIEmitters();
                PawnCheckColli();

                float DiagMod= 5.0f;//1 tile = 5 unit


                int RightSquares= Mathf.FloorToInt(PosibleDis1/DiagMod);            int BackSquares=Mathf.FloorToInt(PosibleDis4/DiagMod);
                int LeftSquares=  Mathf.FloorToInt(PosibleDis2/DiagMod);            int FrontSquares=Mathf.FloorToInt(PosibleDis3/DiagMod);       
                if (gameObject.tag=="Friendly"){ //Friendly move forward
                    forwardpos=new Vector3 (_rigidbody.transform.position.x,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*FrontSquares);
                    if (Rallying){
                        RallyToPlayer(RightSquares,LeftSquares, BackSquares, FrontSquares );
                    }
                } else {
                    forwardpos=new Vector3 (_rigidbody.transform.position.x,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5*BackSquares);
                }
                
                Triangupos=(forwardpos-_rigidbody.transform.position);
                avoidFriendly(forwardpos);
                if (CaptureTargets.Count>0){
                    int randomCap=UnityEngine.Random.Range(0, CaptureTargets.Count);

                    GameObject capTar=CaptureTargets[randomCap]; Vector3 TarPos=capTar.transform.position;

                    Debug.Log(" Is capturing "+ capTar);
                    Debug.Log(" THE TARGET IS AT "+ TarPos);
                    int xmod=0; int zmod=0; bool Modded=false;
                    xmod= Mathf.FloorToInt((TarPos.x-_rigidbody.transform.position.x)/DiagMod);
                    zmod= Mathf.FloorToInt((TarPos.z-_rigidbody.transform.position.z)/DiagMod);

                    forwardpos=new Vector3 (_rigidbody.transform.position.x+5*xmod,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*zmod);

                    Triangupos=(forwardpos-_rigidbody.transform.position);
                    for (int i = 0; i < OtherSide.Length; i++) {
                        if (OtherSide[i].transform.position==CaptureTargets[randomCap].transform.position){//This will allow capture of double stacked pieces
                            captureEnemy(OtherSide[i]);
                        }
                    }
                }
            if (forwardpos!=_rigidbody.transform.position){
                MoveingSound.Play();
            }
            }
        Movement();

    }
    //Pawn Rally


    protected void PawnCheckColli(){
        float maxDist = 7.0f;
        float yOffset = 1.5f;
        RaycastHit hitRight;         RaycastHit hitLeft; 
        RaycastHit hitFront;         RaycastHit hitBack; 

        

        Vector3 right = transform.forward;     Vector3 back= -transform.forward;   
        Vector3 left= transform.forward;     Vector3 front= transform.forward;    

        
        left=Quaternion.Euler(0, -90, 0) *left;     

        right=Quaternion.Euler(0, 90, 0) * right;    

            BoxCollider thisPieceCollider = GetComponent<BoxCollider>();
            bool isHitRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, right, out hitRight, Quaternion.Euler(0, 0, 0), maxDist);
            bool isHitLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, left, out hitLeft, Quaternion.Euler(0, 0, 0), maxDist);

            bool isHitFront = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, front, out hitFront, Quaternion.Euler(0, 0, 0), maxDist);
            bool isHitBack  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, back, out hitBack, Quaternion.Euler(0, 0, 0), maxDist);

            float RhitDis=maxDist;         float LhitDis=maxDist;
            float FhitDis=maxDist;         float BhitDis=maxDist;


            CaptureAt1=false;         CaptureAt2=false; 
            CaptureAt3=false;         CaptureAt4=false; 

            CaptureTargets.Clear();
            string OpposingTag= "Enemy";
            if (gameObject.tag=="Enemy"){OpposingTag="Friendly";}//Colour Side behaviour 

            if (isHitRight)  {   
                GameObject objR = hitRight.collider.gameObject; 
                if(objR.tag==OpposingTag){CaptureAt1=true; CaptureTargets.Add(objR);}  
                RhitDis=hitRight.distance;
            }

            if (isHitLeft)   {   
                GameObject objL = hitLeft.collider.gameObject;     
                if(objL.tag==OpposingTag){CaptureAt2=true; CaptureTargets.Add(objL);}
                LhitDis=hitLeft.distance;
            }

            if (isHitFront) {   
                GameObject objRB = hitFront.collider.gameObject;            
                if(objRB.tag==OpposingTag){CaptureAt3=true;CaptureTargets.Add(objRB);}
                FhitDis=hitFront.distance;
            }
            if (isHitBack)  {   
                GameObject objLB = hitBack.collider.gameObject;               
                if(objLB.tag==OpposingTag){CaptureAt4=true;CaptureTargets.Add(objLB);}
                BhitDis=hitBack.distance;
            }
            Debug.Log(Armynumber+" has Targets: "+CaptureTargets.Count);

            
            PosibleDis1=RhitDis;  PosibleDis2=LhitDis; 
            PosibleDis3=FhitDis;  PosibleDis4=BhitDis;
    }

    public void RallyToPlayer(int BestRight,int BestLeft, int BestBack, int BestFront){//Find the closest move to move to player
        float DistToPlayer= Vector3.Distance(_rigidbody.transform.position, playerTransform.position); 
        //Projecting positionse
        Vector3 forwardpos1=new Vector3 (_rigidbody.transform.position.x+5*BestRight,_rigidbody.transform.position.y,_rigidbody.transform.position.z);
        Vector3 forwardpos2=new Vector3 (_rigidbody.transform.position.x-5*BestLeft,_rigidbody.transform.position.y,_rigidbody.transform.position.z);
        Vector3 forwardpos3=new Vector3 (_rigidbody.transform.position.x,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*BestFront);
        Vector3 forwardpos4=new Vector3 (_rigidbody.transform.position.x,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5*BestBack);
        float DistToPlayer1= Vector3.Distance(forwardpos1, playerTransform.position); float DistToPlayer2= Vector3.Distance(forwardpos2, playerTransform.position); 
        float DistToPlayer3= Vector3.Distance(forwardpos3, playerTransform.position);  float DistToPlayer4= Vector3.Distance(forwardpos4, playerTransform.position); 
        float bestDist=Mathf.Min(DistToPlayer,DistToPlayer1, DistToPlayer2,DistToPlayer3, DistToPlayer4);
        if (bestDist==DistToPlayer){forwardpos=_rigidbody.transform.position;}
        else if (bestDist==DistToPlayer1){forwardpos=forwardpos1;}
        else if (bestDist==DistToPlayer2){forwardpos=forwardpos2;}
        else if (bestDist==DistToPlayer3){forwardpos=forwardpos3;}
        else if (bestDist==DistToPlayer4){forwardpos=forwardpos4;}

    }   

    public void RallySignal(){
        Rallying=!Rallying;
    }

    protected void UpdateMoveQueen(){
        if (transform.position==forwardpos){
            HijackSelect=UnityEngine.Random.Range(0, 2);
        }
        switch (HijackSelect) {
                    case 1: UpdateMoveBishop(); break;
                    case 0: UpdateMoveRook(); break;
                }
    }
        //BISHOP MOVEMENT
        //  X X
        //   X
        //  X X
        //  Left   Right
        //     Center
        //  BRight  Bleft
    protected void UpdateMoveBishop() {
        if ((elapsedTime >= 6.5) && (transform.position==forwardpos)) {
            elapsedTime = 0.0f;         

            ToggleAIEmitters(); killMode=true;// out of stun

            PosibleDis1=39.0f;    PosibleDis2=39.0f;
            PosibleDis3=39.0f;    PosibleDis4=39.0f; 
            BishopCheckColli(); //Determind available movement
            
            float DiagMod= 7.0f;//Modification increase due to diaganol


            int RightSquares= Mathf.FloorToInt(PosibleDis1/DiagMod);         
            int LeftSquares=  Mathf.FloorToInt(PosibleDis2/DiagMod);
            
            int BLeftSquares=Mathf.FloorToInt(PosibleDis3/DiagMod);        
            int BRightSquares=Mathf.FloorToInt(PosibleDis4/DiagMod);
            Debug.Log(gameObject+ " RS " +RightSquares +" LS " +LeftSquares  + " RBS " +BRightSquares +" LBS " +BLeftSquares);   
            
            zax=UnityEngine.Random.Range(0, 2);//the diagnal it should move
            
            //direction
            if (zax==0){
                
                xax=UnityEngine.Random.Range(-BLeftSquares, RightSquares+1);
                

                /*while ((_rigidbody.transform.position.x+5*xax>fieldboundMaxX) || (_rigidbody.transform.position.x+5*xax<fieldboundMinX) || (_rigidbody.transform.position.z+5*xax<fieldboundMinZ) || (_rigidbody.transform.position.z+5*xax>fieldboundMaxZ)){
                    xax=Random.Range(-7, 8);
                    }*/
                forwardpos=new Vector3 (_rigidbody.transform.position.x+5*xax,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*xax);
                } else {
                    
                    xax=UnityEngine.Random.Range(-LeftSquares, BRightSquares+1);
                    
                    /*while ((_rigidbody.transform.position.x+5*xax>fieldboundMaxX) || (_rigidbody.transform.position.x+5*xax<fieldboundMinX) || (_rigidbody.transform.position.z-5*xax<fieldboundMinZ) || (_rigidbody.transform.position.z-5*xax>fieldboundMaxZ)){
                        xax=Random.Range(-7, 8);
                    }*/
                    forwardpos=new Vector3 (_rigidbody.transform.position.x+5*xax,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5*xax);
            }
            //rounding
            Triangupos=(forwardpos-_rigidbody.transform.position);
            
            avoidFriendly(forwardpos);
            if (CaptureTargets.Count>0){
                int randomCap=UnityEngine.Random.Range(0, CaptureTargets.Count);
                
                GameObject capTar=CaptureTargets[randomCap]; Vector3 TarPos=capTar.transform.position;

                
                int xmod=0; int zmod=0; bool Modded=false;
                xmod= Mathf.FloorToInt((TarPos.x-_rigidbody.transform.position.x)/5.0f);
                zmod= Mathf.FloorToInt((TarPos.z-_rigidbody.transform.position.z)/5.0f);

                forwardpos=new Vector3 (_rigidbody.transform.position.x+5*xmod,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*zmod);

                Triangupos=(forwardpos-_rigidbody.transform.position);
                for (int i = 0; i < OtherSide.Length; i++) {
                    if (OtherSide[i].transform.position==CaptureTargets[randomCap].transform.position){//This will allow capture of double stacked pieces
                        captureEnemy(OtherSide[i]);
                    }
                }
            
            }
            if (forwardpos!=_rigidbody.transform.position){
                MoveingSound.Play();
            }
        }
        Movement();
        
    }

    protected void BishopCheckColli(){
            float maxDist = 40.0f;
            float yOffset = 7.0f;
            RaycastHit hit;         
            RaycastHit hitRight;         RaycastHit hitLeft; 
            RaycastHit hitBRight;         RaycastHit hitBLeft; 
            BoxCollider thisPieceCollider = GetComponent<BoxCollider>();
            Vector3 right = transform.forward; 
            Vector3 left= transform.forward; 

            left=Quaternion.Euler(0, -45, 0) *left;
            right=Quaternion.Euler(0, 45, 0) * right;

            Vector3 backRight= transform.forward; 
            Vector3 backLeft= transform.forward; 

            backRight=Quaternion.Euler(0, 180+45, 0) *backRight;
            backLeft=Quaternion.Euler(0, 180-45, 0) *backLeft;
            
            bool isHitRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, right, out hitRight, Quaternion.Euler(0, 0, 0), maxDist);
            bool isHitLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, left, out hitLeft, Quaternion.Euler(0, 0, 0), maxDist);

            bool isHitBRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, backRight, out hitBRight, Quaternion.Euler(0, 0, 0), maxDist);
            bool isHitBLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, backLeft, out hitBLeft, Quaternion.Euler(0, 0, 0), maxDist);

            float RhitDis=40.0f;         float LhitDis=40.0f;
            float RBhitDis=40.0f;        float LBhitDis=40.0f;


            CaptureAt1=false;         CaptureAt2=false; 
            CaptureAt3=false;         CaptureAt4=false; 

            CaptureTargets.Clear();
            string OpposingTag= "Enemy";
            if (gameObject.tag=="Enemy"){OpposingTag="Friendly";}//Colour Side behaviour 

            if (isHitRight)  {   
                GameObject objR = hitRight.collider.gameObject; 
                if(objR.tag==OpposingTag){CaptureAt1=true; CaptureTargets.Add(objR);}  
                RhitDis=hitRight.distance;
            }

            if (isHitLeft)   {   
                GameObject objL = hitLeft.collider.gameObject;     
                if(objL.tag==OpposingTag){CaptureAt2=true; CaptureTargets.Add(objL);}
                LhitDis=hitLeft.distance;
            }

            if (isHitBRight) {   
                GameObject objRB = hitBRight.collider.gameObject;            
                if(objRB.tag==OpposingTag){CaptureAt3=true;CaptureTargets.Add(objRB);}
                RBhitDis=hitBRight.distance;
            }
            if (isHitBLeft)  {   
                GameObject objLB = hitBLeft.collider.gameObject;               
                if(objLB.tag==OpposingTag){CaptureAt4=true;CaptureTargets.Add(objLB);}
                LBhitDis=hitBLeft.distance;
            }
            Debug.Log(Armynumber+" has Targets: "+CaptureTargets.Count);

            
            PosibleDis1=RhitDis;  PosibleDis2=LhitDis; 
            PosibleDis3=RBhitDis; PosibleDis4=LBhitDis;

            /*class HitDis{
                float RhitDisC;    float LhitDisC;
                float RBhitDisC;   float LBhitDisC;
            }
            HitDis.RhitDisC=RhitDis;            HitDis.LhitDisC=LhitDis;
            HitDis.RBhitDisC=RBhitDis;          HitDis.LBhitDisC=LBhitDis;*/

            //return(isHitRight,isHitLeft,isHitBRight,isHitBLeft);
    }
    //Knight MOVEMENT
        //  0        1
        // 7          2
        //    Knight  
        // 6          3      
        //  5        4
        //  
    protected void UpdateMoveKnight(){
        if ((elapsedTime >= 5.5f) && (transform.position==forwardpos)) {
            KnightPhase=0;                 killMode=true;         ToggleAIEmitters();

            elapsedTime = 0.0f;
            KnightAllowedJump();
            float PlayerPotenDist= Vector3.Distance(playerTransform.position, forwardpos);
            float CurrentDist= Vector3.Distance(playerTransform.position, _rigidbody.transform.position);


            if ((PlayerPotenDist>CurrentDist)&&(gameObject.tag=="Enemy")){//Enemies tend to chase after player
                KnightAllowedJump();
                if ((PlayerPotenDist>CurrentDist)&&(gameObject.tag=="Enemy")){
                    KnightAllowedJump();
                    }
            }
            KnightReturnJump(); //Rejump until it is within acceptable distance
            }
        //Start Jumping
        //                2  <--1   
        //                      |
        //      Target => 3     0 <=Start
        switch (KnightPhase) {
            case 0: 
                Vector3 UpperFowardPos= new Vector3 (_rigidbody.transform.position.x,16.5f,_rigidbody.transform.position.z);
                _rigidbody.transform.position= Vector3.MoveTowards(_rigidbody.transform.position, UpperFowardPos, 0.05f);
                if (_rigidbody.transform.position.y==16.5f){KnightPhase+=1;}
                 int gallop=UnityEngine.Random.Range(0, 400);
                 if (gallop>375){ MoveingSound.Play();}
            break;
            case 1:
                Vector3 UpperFowardPos2= forwardpos + new Vector3 (0.0f, 9.5f,0.0f);
                _rigidbody.transform.position= Vector3.MoveTowards(_rigidbody.transform.position, UpperFowardPos2, 0.05f);
                if (_rigidbody.transform.position==UpperFowardPos2){KnightPhase+=1;}
            break;
            case 2:
                _rigidbody.transform.position= Vector3.MoveTowards(_rigidbody.transform.position, forwardpos, 0.05f);
                if (_rigidbody.transform.position==forwardpos){KnightPhase+=1; knightTremor();}
            break;
        }
    }
    protected void KnightAllowedJump(){
                zax=UnityEngine.Random.Range(0, 8);
                switch (zax) {
                    case 0: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x-5,_rigidbody.transform.position.y,_rigidbody.transform.position.z+10); 
                        break;
                    case 1:                     
                        forwardpos= new Vector3 (_rigidbody.transform.position.x+5,_rigidbody.transform.position.y,_rigidbody.transform.position.z+10); 
                        break;
                    case 2: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x+10,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5); 
                        break;
                    case 3: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x+10,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5); 
                        break;
                    case 4: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x+5,_rigidbody.transform.position.y,_rigidbody.transform.position.z-10); 
                        break;
                    case 5: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x-5,_rigidbody.transform.position.y,_rigidbody.transform.position.z-10); 
                        break;
                    case 6: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x-10,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5); 
                        break;
                    case 7: 
                        forwardpos= new Vector3 (_rigidbody.transform.position.x-10,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5); 
                        break;
                
            }
            
    }
    protected void KnightReturnJump(){
                float farFromHome=  Vector3.Distance(centralPoint.transform.position, _rigidbody.transform.position); 
                if (farFromHome>50.5f){
                    float wayHome=51.0f; Vector3 tempWardpost; 
                    for (int i = 0; i < 8; i++) {
                        switch (i) {
                            case 0: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x-5,_rigidbody.transform.position.y,_rigidbody.transform.position.z+10); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            case 1:                     
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x+5,_rigidbody.transform.position.y,_rigidbody.transform.position.z+10); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}

                                break;
                            case 2: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x+10,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            case 3: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x+10,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            case 4: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x+5,_rigidbody.transform.position.y,_rigidbody.transform.position.z-10); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            case 5: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x-5,_rigidbody.transform.position.y,_rigidbody.transform.position.z-10); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            case 6: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x-10,_rigidbody.transform.position.y,_rigidbody.transform.position.z-5); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            case 7: 
                                tempWardpost= new Vector3 (_rigidbody.transform.position.x-10,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5); 
                                if (Vector3.Distance(centralPoint.transform.position, tempWardpost)<wayHome ){wayHome=Vector3.Distance(centralPoint.transform.position, tempWardpost); forwardpos=tempWardpost;}
                                break;
                            }
                    } 
                }
                
            
    }
    protected void knightTremor(){ //AoE Stunning
        tremmorSound.Play();
        for (int i = 0; i < OtherSide.Length; i++) {
            float tremmorDist= Vector3.Distance(OtherSide[i].transform.position,_rigidbody.transform.position);
            if (tremmorDist<10.0f){captureEnemy(OtherSide[i]);}
        }
        float PlayertremmorDist= Vector3.Distance(playerTransform.position,_rigidbody.transform.position);
        if ((PlayertremmorDist<10.0f) && (gameObject.tag=="Enemy")){playerTanker.SendMessage("shockWaveLostAmmo");}

        }

    //Rook MOVEMENT
        //   X
        //  XXX
        //   X
        //      Front   
        // Left Center  Right
        //      Back
    protected void UpdateMoveRook(){
        if ((elapsedTime >= 6.5f) && (transform.position==forwardpos)) {
            elapsedTime = 0.0f;         ToggleAIEmitters();

            killMode=true;
            PosibleDis1=49.0f;    PosibleDis2=49.0f;
            PosibleDis3=49.0f;    PosibleDis4=49.0f; 
            RookCheckColli();
            float DiagMod= 5.0f;//1 tile = 5 unit


            int RightSquares= Mathf.FloorToInt(PosibleDis1/DiagMod);         
            int LeftSquares=  Mathf.FloorToInt(PosibleDis2/DiagMod);
            
            int FrontSquares=Mathf.FloorToInt(PosibleDis3/DiagMod);       
            int BackSquares=Mathf.FloorToInt(PosibleDis4/DiagMod);

            int MimalMove=-9;
            int MaxilMove=9;
            zax=UnityEngine.Random.Range(0, 2);
            if (zax==0){
                
                xax=UnityEngine.Random.Range(-BackSquares, FrontSquares);
                

                /*while ((_rigidbody.transform.position.x+5*xax>fieldboundMaxX) || (_rigidbody.transform.position.x+5*xax<fieldboundMinX) || (_rigidbody.transform.position.z+5*xax<fieldboundMinZ) || (_rigidbody.transform.position.z+5*xax>fieldboundMaxZ)){xax=Random.Range(-7, 8);}*/
                forwardpos=new Vector3 (_rigidbody.transform.position.x,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*xax);
                } else {
                    
                    xax=UnityEngine.Random.Range(-LeftSquares, RightSquares);
                    
                    /*while ((_rigidbody.transform.position.x+5*xax>fieldboundMaxX) || (_rigidbody.transform.position.x+5*xax<fieldboundMinX) || (_rigidbody.transform.position.z-5*xax<fieldboundMinZ) || (_rigidbody.transform.position.z-5*xax>fieldboundMaxZ)){xax=Random.Range(-7, 8);}*/
                    forwardpos=new Vector3 (_rigidbody.transform.position.x+5*xax,_rigidbody.transform.position.y,_rigidbody.transform.position.z);
            }
            Triangupos=(forwardpos-_rigidbody.transform.position);
            avoidFriendly(forwardpos);
            if (CaptureTargets.Count>0){
                int randomCap=UnityEngine.Random.Range(0, CaptureTargets.Count);

                GameObject capTar=CaptureTargets[randomCap]; Vector3 TarPos=capTar.transform.position;

    
                int xmod=0; int zmod=0; bool Modded=false;
                xmod= Mathf.FloorToInt((TarPos.x-_rigidbody.transform.position.x)/DiagMod);
                zmod= Mathf.FloorToInt((TarPos.z-_rigidbody.transform.position.z)/DiagMod);

                forwardpos=new Vector3 (_rigidbody.transform.position.x+5*xmod,_rigidbody.transform.position.y,_rigidbody.transform.position.z+5*zmod);

                Triangupos=(forwardpos-_rigidbody.transform.position);
                for (int i = 0; i < OtherSide.Length; i++) {
                    if (OtherSide[i].transform.position==CaptureTargets[randomCap].transform.position){//This will allow capture of double stacked pieces
                        captureEnemy(OtherSide[i]);
                    }
                }
            }
        if (forwardpos!=_rigidbody.transform.position){// Activate sound
                MoveingSound.Play();
            }
        }
        Movement();

    }
    protected void RookCheckColli(){
            float maxDist = 50.0f;
            float yOffset = 0.5f;
            RaycastHit hit;
            RaycastHit hitRight;         RaycastHit hitLeft; 
            RaycastHit hitFront;         RaycastHit hitBack; 

            GameObject friendly = GameObject.FindGameObjectWithTag("Friendly");
            Transform friendTransform = friendly.transform;
            BoxCollider thisPieceCollider = GetComponent<BoxCollider>();
            BoxCollider friendlyCollider = friendly.GetComponent<BoxCollider>();

            float distance = Vector3.Distance(transform.position, friendTransform.position);
            //Debug.Log("Distance = " + (distance - (myCollider.size.z / 2) - (enemyCollider.size.z/2)));

            Vector3 tankCastSource = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);


            Vector3 right = transform.forward; 
            Vector3 left= transform.forward; 

            left=Quaternion.Euler(0, -90, 0) *left;
            right=Quaternion.Euler(0, 90, 0) * right;

            Vector3 front= transform.forward; 
            Vector3 back= -transform.forward; 
            
            bool isHitRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, right, out hitRight, Quaternion.Euler(0, 0, 0), maxDist);
            bool isHitLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, left, out hitLeft, Quaternion.Euler(0, 0, 0), maxDist);

            bool isHitFront = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, front, out hitFront, Quaternion.Euler(0, 0, 0), maxDist);
            bool isHitBack  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, back, out hitBack, Quaternion.Euler(0, 0, 0), maxDist);

            float RhitDis=maxDist;         float LhitDis=maxDist;
            float FhitDis=maxDist;         float BhitDis=maxDist;


            CaptureAt1=false;         CaptureAt2=false; 
            CaptureAt3=false;         CaptureAt4=false; 

            CaptureTargets.Clear();
            string OpposingTag= "Enemy";
            if (gameObject.tag=="Enemy"){OpposingTag="Friendly";}//Colour Side behaviour 

            if (isHitRight)  {   
                GameObject objR = hitRight.collider.gameObject; 
                if(objR.tag==OpposingTag){CaptureAt1=true; CaptureTargets.Add(objR);}  
                if((objR.tag=="Player") && (gameObject.tag=="Enemy")){TankSpawn();}
                RhitDis=hitRight.distance;
            }

            if (isHitLeft)   {   
                GameObject objL = hitLeft.collider.gameObject;     
                if(objL.tag==OpposingTag){CaptureAt2=true; CaptureTargets.Add(objL);}
                if((objL.tag=="Player") && (gameObject.tag=="Enemy")){TankSpawn();}
                LhitDis=hitLeft.distance;
            }

            if (isHitFront) {   
                GameObject objRB = hitFront.collider.gameObject;            
                if(objRB.tag==OpposingTag){CaptureAt3=true;CaptureTargets.Add(objRB);}
                if((objRB.tag=="Player") && (gameObject.tag=="Enemy")){TankSpawn();}
                FhitDis=hitFront.distance;
            }
            if (isHitBack)  {   
                GameObject objLB = hitBack.collider.gameObject;               
                if(objLB.tag==OpposingTag){CaptureAt4=true;CaptureTargets.Add(objLB);}
                if((objLB.tag=="Player") && (gameObject.tag=="Enemy")){TankSpawn();}
                BhitDis=hitBack.distance;
            }
            
            PosibleDis1=RhitDis;  PosibleDis2=LhitDis; 
            PosibleDis3=FhitDis;  PosibleDis4=BhitDis;

            /*class HitDis{
                float RhitDisC;    float LhitDisC;
                float RBhitDisC;   float LBhitDisC;
            }
            HitDis.RhitDisC=RhitDis;            HitDis.LhitDisC=LhitDis;
            HitDis.RBhitDisC=RBhitDis;          HitDis.LBhitDisC=LBhitDis;*/

            //return(isHitRight,isHitLeft,isHitBRight,isHitBLeft);
    }

    protected void TankSpawn(){
        if ((Spawnder) & (SpawnderLocation))
                Instantiate(Spawnder, SpawnderLocation.transform.position, SpawnderLocation.transform.rotation);
    }

        //move and snap
    protected void Movement() {
        
        float distance = Vector3.Distance(_rigidbody.transform.position, forwardpos);
        if ((distance<0.85f)||(forwardpos==_rigidbody.transform.position)){
            _rigidbody.transform.position=forwardpos;
            Triangupos=new Vector3 (0.0f,0.0f,0.0f);
            float relavXFloat =(12.5f+transform.position.x)/5.0f; 
            relativeX= (int)relavXFloat;

            float relavZFloat =(12.5f+transform.position.z)/5.0f; 
            relativeZ= (int)relavZFloat;
            //promote a pawn
            if ((_rigidbody.transform.position.z< promotetionZ) &&(TypeChess==PieceType.Pawn) && (gameObject.tag=="Enemy") ){
                TypeChess=PieceType.Rook;
                transformPawn();
            }
        } else {
            float approacherSpeed= 0.8f*distance; //dynamic speed
            //_rigidbody.MovePosition(_rigidbody.transform.position+Triangupos*approacherSpeed*Time.deltaTime);
            _rigidbody.position=  Vector3.MoveTowards(transform.position, forwardpos, 20.75f* Time.deltaTime);
        }

        if (killMode){//Capture in collision
            float killRange=0.75f;
                if (gameObject.tag=="Enemy"){killRange+=1.0f;}//Ally has less capture range than enemy in case they go toward the same place
            for (int i = 0; i < OtherSide.Length; i++) {
                if (killRange>Vector3.Distance(_rigidbody.transform.position,  OtherSide[i].transform.position) ){
                    OtherSide[i].SendMessage("gotDefeated");
                }
            } 
        }
    }
    

    public void transformPawn(){
            NewShell.SendMessage("BeginTransform");

    }

    protected void avoidFriendly(Vector3 Destination) {
        GameObject[] CurrentSide;
        if (gameObject.tag=="Enemy"){
                CurrentSide=RedSide;
            } else {
                CurrentSide=WhiteSide;
            }
        
        for (int i = 0; i < CurrentSide.Length; i++) {
            float distanceRadAvoid = Vector3.Distance(CurrentSide[i].transform.position, forwardpos);
            if ((i!=Armynumber) && (distanceRadAvoid<=1.0f)){
                forwardpos=transform.position;
                Triangupos=new Vector3 (0.0f,0.0f,0.0f);
                Debug.Log("Refrain");  
            }
            } 
    }


    public void captureEnemy(GameObject EnemyTarget) {
        //Debug.Log(gameObject.tag+ " "+ Armynumber+ " Attempting capture "+EnemyTarget.tag+" "+ EnemyTarget+ " at "+ EnemyTarget.transform.position);
        Unit EnemyUnit = (Unit) EnemyTarget.GetComponent(typeof(Unit));
        //EnemyTarget.gotCaptured(-7.5f);
        EnemyTarget.SendMessage("gotCaptured", 7.0f);

    }

    public void gotCaptured(float stunTime) {
            elapsedTime=0.0f-stunTime;
            killMode=false;// unit is now vulnable
            ToggleAIEmitters();
            Debug.Log(gameObject+" Unit Stunned");
        }

    public void boosted(float boostTime) {
            elapsedTime+=boostTime;
            killMode=true;// unit is now powered
        }

    public void gotDefeated() {
        IsActive=false;
        transform.position=new Vector3(17.5f, -37.0f, 2.5f);
        if (gameObject.tag=="Enemy"){ 
            playerTanker.SendMessage("getVitoryPoint", UnitValue);
            if (NewShell){
                NewShell.SendMessage("RIP");
            }}
        else{playerTanker.SendMessage("getDefeatPoint", UnitValue);}
        }

    public void plateBreak() {//HP control
        HP-=1;
        if (HP >=0){PlatesEnemmy[HP].SendMessage("BreakDown");
        }
        if (HP<0){gotDefeated();}
        }


    public void RoundEndedUnit() {IsActive=false;}
    public void ToggleAIEmitters()
    {//Visualise Stunstate
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (killMode)
        {
            ps.Play();
        } else if (!killMode)
        {
            ps.Stop();

        }
    }

}

