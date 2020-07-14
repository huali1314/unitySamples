using System.Collections;
using System.Collections.Generic;
using RVO;
using UnityEngine;
using Unity.Burst;
public class RVOManager : UnitySingleton<RVOManager>
{
    /* Store the goals of the agents. */
    //存储代理的目标
    IList<RVO.Vector2> goals;
    IList<RVO.Vector2> obstacle = new List<RVO.Vector2>();
    System.Random random;
    private void Awake()
    {
        goals = new List<RVO.Vector2>();
         /** Random number generator. */
#if RVO_SEED_RANDOM_NUMBER_GENERATOR
            random = new Random();
#else
        random = new System.Random(0);
#endif
         setupScenario();
    }

    // Update is called once per frame
    void Update()
    {
        if(!reachedGoal())
        {
#if RVO_OUTPUT_TIME_AND_POSITIONS
                blocks.updateVisualization();
#endif
            setPreferredVelocities();
            Simulator.Instance.doStep();
        }
    }
   
    public int AddAgent(RVO.Vector2 position,RVO.Vector2 goalPos)
    {
        goals.Add(goalPos);
        //添加代理，并设置他们的位置，保存目标到环境的一侧
        return Simulator.Instance.addAgent(position);
        
    }
    public void AddObstacles(RVO.Vector2 position)
    {
        obstacle.Add(position);
    }
    void setupScenario()
    {
        /* Specify the global time step of the simulation. */
        //设置模拟器的全局步进时间
        Simulator.Instance.setTimeStep(0.25f);

        /*
         * Specify the default parameters for agents that are subsequently
         * added.
         */
        //设置导航代理默认参数
        Simulator.Instance.setAgentDefaults(5.0f, 10, 5.0f, 5.0f, 1.0f, 2.0f, new RVO.Vector2(0.0f, 0.0f));
        Simulator.Instance.addObstacle(obstacle);
        /*
         * Process the obstacles so that they are accounted for in the
         * simulation.
         */
        //处理障碍物以便在仿真中模拟
        Simulator.Instance.processObstacles();
    }

#if RVO_OUTPUT_TIME_AND_POSITIONS
        void updateVisualization()
        {
            /* Output the current global time. */
            //输出当前全局时间
            Console.Write(Simulator.Instance.getGlobalTime());

            /* Output the current position of all the agents. */
            //输出当前所有代理的位置
            for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
            {
                Console.Write(" {0}", Simulator.Instance.getAgentPosition(i));
            }

            Console.WriteLine();
        }
#endif
    [BurstCompile]
    //设置首选速度
    void setPreferredVelocities()
    {
        /*
         * Set the preferred velocity to be a vector of unit magnitude
         * (speed) in the direction of the goal.
         */
        //将首选速度设置为目标方向上的单位向量
        for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
        {
            //得到寻路代理到达目标位置的速度，包括大小和方向，这里我们只关心方向，会进行归一化处理
            RVO.Vector2 goalVector = goals[i] - Simulator.Instance.getAgentPosition(i);
            //如果向量的大小大于1，进行归一化处理
            if (RVOMath.absSq(goalVector) > 1.0f)
            {
                goalVector = RVOMath.normalize(goalVector);
            }
            //设置寻路代理的首选方向，即从代理位置到达目标位置的最佳的速度方向
            Simulator.Instance.setAgentPrefVelocity(i, goalVector);

            /* Perturb a little to avoid deadlocks due to perfect symmetry. */
            float angle = (float)random.NextDouble() * 2.0f * (float)System.Math.PI;
            float dist = (float)random.NextDouble() * 0.0001f;

            Simulator.Instance.setAgentPrefVelocity(i, Simulator.Instance.getAgentPrefVelocity(i) +
                dist * new RVO.Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)));
        }
    }
    [BurstCompile]
    bool reachedGoal()
    {
        /* Check if all agents have reached their goals. */
        for (int i = 0; i < Simulator.Instance.getNumAgents(); ++i)
        {
            if (RVOMath.absSq(Simulator.Instance.getAgentPosition(i) - goals[i]) > 400f)
            {
                return false;
            }
        }

        return true;
    }
}
