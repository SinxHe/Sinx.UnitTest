# 关于面向对象抽象的理解(2)

杯子是一个容器, 水桶也是一个容器.  
容器是什么? 能够盛放东西的就是容器. 但是盛放的是什么东西, 东西的多少, 是否是流动性的并没有确切说明, 抽象出了能力, 省略了细枝末节.  
那杯子和水桶有什么异同呢?  

1.相同

  * 他们都**能**盛放东西, is 容器
  
2.不同

 * 盛放的量不同, 水桶**能**比杯子盛放的多

在编程领域, 如果水桶和杯子从容器中继承来, 那水桶和杯子必然有盛放东西的能力. 容器给了水桶和杯子盛放东西的能力. 水桶和杯子给了容器盛放物品和多少的限制.

苹果和香蕉是水果, 水果是什么, 有果皮, 果核, 果肉的东西就是水果. 水果规定了有什么, 但是没有规定果皮厚度, 果核大小, 果肉酸甜. 每个具体的水果来规定具体的大小. 

对与抽象来说, 我们只抽取一堆事物的共有概念. 不关注具体细节.

拿编程领域的"容器"来说:  
IEnumerable 规定, 集合有遍历自己每个元素的能力.  
里面多少个元素?  怎么遍历? 能否并发? 能不能增加/减少? 我是在抽象, 不care.  
ICollection : IEnumerable 进一步规定了集合的大小. 好吧, 我开始关注有多少个元素了. 我好像也开始关注增加减少了.
IList : ICollection 规定, 集合可以在元素之间的任意地方插入添加.
IReadOnlyCollection : IEnumerable 规定, 集合只有大小, 只能读
IDictionary : IEnumerable 规定, 遍历集合的时候可以根据Key值进行快速查找  
IGrouping : IEnumerable 规定, 可以给一个集合进行Key值标识  
上面列举了一大堆能力, 好像我们有了不同的"容器", 下面开始"铸造杯子"了.  

List : IList -> 支持IList规定的能力, 支持IEnumerable规定的能力. 但是对于使用者来说, 动态数组也好, 链表也罢. 我只需要知道你有我需要的能力就行了. 在遍历一次的时候, 我用IEnumerable接收, 在遍历多次的时候, 我用ICollection接收, 需要插入的时候, 用IList接收, 我只关注我需要你干什么, 而不关注你是什么.  

那么问题来了, 对于上面的一大把水果来说, 就好像是编程领域的"DTO"对象, 他们没有多少"动作", 只能通过一堆名词来进行共有内容的总结, 然后对这些名词集合用一个名词"水果"进行概括. 这个正好对应我目前遇见的问题. 我是把这一堆名词集合到一起赋予一个新的概念呢. 该是将这些名词详细划分到各个类, 然后进行链式继承呢(C#不支持多继承). 还是放到多个接口中, 进行接口的多继承?
```CSharp
class Auto 
{
    int AutoId;
    string CnName;
    string EnName;
    string ViewWords;
    IList<VendorBrand> VendorBrands;
}
class Brand
{
    int BrandId;
    string CnName;
    string EnName;
    string ViewWords;
}
class Vendor
{
    int VendorId;
    string CnName;
    string EnName;
    string ViewWords;
}
```